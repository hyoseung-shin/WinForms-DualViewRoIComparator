using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DualViewRoiComparator.Core;
using DualViewRoiComparator.Heatmap;
using DualViewRoiComparator.Persistence;
using DualViewRoiComparator.UI;
using DualViewRoiComparator.Forms;

namespace DualViewRoiComparator
{
    /// <summary>
    /// Main window (single-video). One source video is shown in three side-by-side panels:
    /// original (left), ROI-overlaid (center), and the accumulated spatial-density heatmap
    /// (right). The chart compares ROI vs background estimated bits, and analysis sessions are
    /// persisted as JSON + a heatmap PNG with full CRUD.
    /// Designer-generated fields / components / Dispose(bool) live in MainForm.Designer.cs.
    /// </summary>
    public partial class MainForm : Form
    {
        private static readonly Color RoiColor = Color.OrangeRed;

        private VideoSourceManager _video;
        private PlaybackController _playback;
        private RoiAnalyzer _analyzer;
        private HeatmapAccumulator _heatmap;
        private ChartManager _chart;
        private SessionManager _sessions;

        private Bitmap _prev;            // clean previous frame (diff reference)
        private bool _suppressSeekEvent;

        public MainForm()
        {
            InitializeComponent();
        }

        // ----------------------------------------------------------------- lifecycle

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                _analyzer = new RoiAnalyzer();
                _analyzer.Threshold = trackBarThreshold.Value;
                _heatmap = new HeatmapAccumulator(64, 36);
                _chart = new ChartManager(chartBits);
                _playback = new PlaybackController();
                _playback.Tick += OnPlaybackTick;
                _sessions = new SessionManager(AppDomain.CurrentDomain.BaseDirectory);

                lblBackend.Text = "Backend: " + _analyzer.BackendName;
                lblThreshold.Text = trackBarThreshold.Value.ToString();

                RefreshSessionList();
                UpdateControlState();
                SetStatus("준비됨 - [파일 > 영상 열기]로 영상을 불러오세요.");
            }
            catch (Exception ex)
            {
                ShowError("초기화 중 오류가 발생했습니다.", ex);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_playback != null) { _playback.Pause(); _playback.Dispose(); }
                if (_video != null) _video.Dispose();
                DisposeBitmap(ref _prev);
                DisposePictureImage(pictureBoxOriginal);
                DisposePictureImage(pictureBoxRoi);
                DisposePictureImage(pictureBoxHeatmap);
            }
            catch { /* never block shutdown */ }
        }

        // ----------------------------------------------------------------- file/help menu

        private void menuOpenVideo_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = string.Empty;
            if (openFileDialog1.ShowDialog(this) != DialogResult.OK) return;

            string path = openFileDialog1.FileName;
            bool isYuv = string.Equals(Path.GetExtension(path), ".yuv", StringComparison.OrdinalIgnoreCase);

            try
            {
                if (_playback != null) _playback.Pause();
                if (_video != null) { _video.Dispose(); _video = null; }
                _video = new VideoSourceManager();

                string error;
                bool ok;
                if (isYuv)
                {
                    using (YuvSpecDialog dlg = new YuvSpecDialog())
                    {
                        if (dlg.ShowDialog(this) != DialogResult.OK) { _video.Dispose(); _video = null; return; }
                        ok = _video.LoadYuv(path, dlg.FrameWidth, dlg.FrameHeight, dlg.Fps, out error);
                    }
                }
                else
                {
                    ok = _video.LoadVideo(path, out error);
                }

                if (!ok)
                {
                    _video.Dispose(); _video = null;
                    MessageBox.Show(this, error, "영상 로드 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetStatus("영상 로드 실패.");
                    UpdateControlState();
                    return;
                }

                _playback.SetFps(_video.Fps);
                DisposeBitmap(ref _prev);
                _heatmap.Reset();
                _chart.Reset();

                _suppressSeekEvent = true; trackBarSeek.Value = 0; _suppressSeekEvent = false;
                ReadAndProcess(true);          // render first frame (zero motion)
                btnPlayPause.Text = "▶ 재생";
                UpdateControlState();
                SetStatus(string.Format("로드 완료 - {0} 프레임, {1:0.#} fps, 처리 해상도 {2}x{3}, Backend={4}",
                    _video.TotalFrames, _video.Fps,
                    _video.ProcessingSize.Width, _video.ProcessingSize.Height, _analyzer.BackendName));
            }
            catch (Exception ex)
            {
                ShowError("영상을 여는 중 오류가 발생했습니다.", ex);
            }
        }

        private void menuExit_Click(object sender, EventArgs e) { Close(); }

        private void menuHelpInfo_Click(object sender, EventArgs e)
        {
            string msg =
                "[지원 형식]\n" +
                "- 컨테이너: mp4, avi, mov, mkv, wmv (시스템 코덱 사용)\n" +
                "- raw YUV: .yuv (8비트 I420 / YUV420p 가정)\n\n" +
                "[raw .yuv 주의사항]\n" +
                "- 헤더가 없으므로 열 때 가로/세로/FPS를 직접 입력해야 합니다.\n" +
                "- 해상도가 실제와 다르면 영상이 깨지거나 로드가 실패합니다.\n\n" +
                "[크기/성능 안내]\n" +
                "- 모든 프레임은 분석 시 최대 640x360으로 다운스케일됩니다(종횡비 유지).\n" +
                "- 매우 긴 영상이나 고해상도 raw YUV(수 GB)는 메모리/재생 성능에 영향을 줄 수 있습니다.\n" +
                "- 권장: 길이 수 분 이내, 파일 크기 1GB 이하.\n\n" +
                "[세션 저장]\n" +
                "- 세션은 실행 폴더의 Sessions\\ 아래에 JSON과 히트맵 PNG로 함께 저장됩니다.";
            MessageBox.Show(this, msg, "파일 업로드 안내 및 주의사항",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ----------------------------------------------------------------- reset menu

        private void menuResetHeatmap_Click(object sender, EventArgs e)
        {
            if (_heatmap == null) return;
            _heatmap.Reset();
            RefreshHeatmapDisplay();
            SetStatus("히트맵을 초기화했습니다.");
        }

        private void menuResetChart_Click(object sender, EventArgs e)
        {
            if (_chart == null) return;
            _chart.Reset();
            SetStatus("그래프를 초기화했습니다.");
        }

        // ----------------------------------------------------------------- playback

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            if (!IsVideoReady()) return;
            _playback.Toggle();
            btnPlayPause.Text = _playback.IsPlaying ? "|| 일시정지" : "▶ 재생";
            SetStatus(_playback.IsPlaying ? "재생 중..." : "일시정지.");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (!IsVideoReady()) return;
            _playback.Pause();
            btnPlayPause.Text = "▶ 재생";
            JumpToFrame(0);
            SetStatus("정지 - 처음으로 이동했습니다.");
        }

        private void btnPrevFrame_Click(object sender, EventArgs e)
        {
            if (!IsVideoReady()) return;
            _playback.Pause();
            btnPlayPause.Text = "▶ 재생";
            JumpToFrame(_video.CurrentIndex - 1);
        }

        private void btnNextFrame_Click(object sender, EventArgs e)
        {
            if (!IsVideoReady()) return;
            _playback.Pause();
            btnPlayPause.Text = "▶ 재생";
            if (!ReadAndProcess(false)) SetStatus("마지막 프레임입니다.");
        }

        private void trackBarSeek_Scroll(object sender, EventArgs e)
        {
            if (_suppressSeekEvent || !IsVideoReady()) return;
            _playback.Pause();
            btnPlayPause.Text = "▶ 재생";
            int total = _video.TotalFrames;
            if (total <= 0) return;
            long target = (long)trackBarSeek.Value * (total - 1) / 100;
            JumpToFrame((int)target);
        }

        private void trackBarThreshold_Scroll(object sender, EventArgs e)
        {
            int value = trackBarThreshold.Value;
            lblThreshold.Text = value.ToString();
            if (_analyzer != null) _analyzer.Threshold = value;
        }

        private void OnPlaybackTick(object sender, EventArgs e)
        {
            if (!IsVideoReady()) { _playback.Pause(); return; }
            if (!ReadAndProcess(false))
            {
                _playback.Pause();
                btnPlayPause.Text = "▶ 재생";
                SetStatus("재생 완료 - 마지막 프레임에 도달했습니다.");
            }
        }

        // ----------------------------------------------------------------- pipeline

        private void JumpToFrame(int target)
        {
            if (!IsVideoReady()) return;
            if (target < 0) target = 0;
            if (!_video.Seek(target)) return;
            ReadAndProcess(true);
        }

        /// <summary>
        /// Reads the next frame, analyses ROI, and fills the 3 panels + chart. Returns false
        /// at end-of-stream. When <paramref name="resetPrev"/> is true (discontinuous jump),
        /// the diff reference is reset so no spurious full-frame motion is produced.
        /// </summary>
        private bool ReadAndProcess(bool resetPrev)
        {
            Bitmap frame;
            int index;
            if (!_video.ReadNextFrame(out frame, out index))
            {
                if (frame != null) frame.Dispose();
                return false;
            }

            try
            {
                if (resetPrev)
                {
                    DisposeBitmap(ref _prev);
                    _prev = (Bitmap)frame.Clone();
                }

                RoiResult result = _analyzer.Analyze(_prev, frame);

                // Panel 1: original (no overlay). Panel 2: ROI overlay.
                SetPictureImage(pictureBoxOriginal, (Bitmap)frame.Clone());
                SetPictureImage(pictureBoxRoi, DrawRoi(frame, result.BoundingBoxes, RoiColor));

                // Panel 3: heatmap.
                if (result.MotionMask != null) _heatmap.Accumulate(result.MotionMask);
                RefreshHeatmapDisplay();

                _chart.AddPoint(index, result.EstimatedBitsRoi, result.EstimatedBitsBackground);

                DisposeBitmap(ref _prev);
                _prev = frame;     // keep clean current frame as next reference
                frame = null;

                UpdateFrameInfo(index);
                UpdateSeekBar(index);
                return true;
            }
            finally
            {
                if (frame != null) frame.Dispose();
            }
        }

        private static Bitmap DrawRoi(Bitmap source, Rectangle[] boxes, Color color)
        {
            Bitmap copy = (Bitmap)source.Clone();
            if (boxes != null && boxes.Length > 0)
            {
                using (Graphics g = Graphics.FromImage(copy))
                using (Pen pen = new Pen(color, 2f))
                {
                    for (int i = 0; i < boxes.Length; i++)
                    {
                        Rectangle r = boxes[i];
                        if (r.Width > 0 && r.Height > 0) g.DrawRectangle(pen, r);
                    }
                }
            }
            return copy;
        }

        private void RefreshHeatmapDisplay()
        {
            if (_heatmap == null) return;
            int w = Math.Max(2, pictureBoxHeatmap.Width);
            int h = Math.Max(2, pictureBoxHeatmap.Height);
            SetPictureImage(pictureBoxHeatmap, _heatmap.GetHeatmapBitmap(w, h));
        }

        // ----------------------------------------------------------------- sessions

        private void btnSaveSession_Click(object sender, EventArgs e)
        {
            if (_sessions == null) return;
            if (_video == null || !_video.IsLoaded)
            {
                MessageBox.Show(this, "저장할 분석 데이터가 없습니다. 먼저 영상을 분석하세요.",
                    "저장 불가", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SessionEditForm dialog = new SessionEditForm("세션 저장", string.Empty, string.Empty))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    SessionData data = BuildSessionData(dialog.SessionName, dialog.Memo);
                    _sessions.Create(data);
                    RefreshSessionList();
                    SetStatus("세션을 저장했습니다 (JSON + 히트맵 PNG): " + data.Name);
                }
                catch (SessionPersistenceException spex)
                {
                    MessageBox.Show(this, spex.Message, "저장 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex) { ShowError("세션 저장 중 오류가 발생했습니다.", ex); }
            }
        }

        private void btnUpdateSession_Click(object sender, EventArgs e)
        {
            string id = GetSelectedSessionId();
            if (id == null) { WarnSelect("수정"); return; }
            if (_video == null || !_video.IsLoaded)
            {
                MessageBox.Show(this, "갱신할 분석 데이터가 없습니다. 먼저 영상을 분석하세요.",
                    "수정 불가", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string currentName = listViewSessions.SelectedItems[0].Text;
            using (SessionEditForm dialog = new SessionEditForm("세션 수정", currentName, string.Empty))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    SessionData data = BuildSessionData(dialog.SessionName, dialog.Memo);
                    _sessions.Update(id, data);
                    RefreshSessionList();
                    SetStatus("세션을 수정했습니다: " + data.Name);
                }
                catch (SessionPersistenceException spex)
                {
                    MessageBox.Show(this, spex.Message, "수정 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex) { ShowError("세션 수정 중 오류가 발생했습니다.", ex); }
            }
        }

        private void btnLoadSession_Click(object sender, EventArgs e)
        {
            string id = GetSelectedSessionId();
            if (id == null) { WarnSelect("불러올"); return; }

            try
            {
                SessionData data = _sessions.ReadById(id);
                if (data == null)
                {
                    MessageBox.Show(this, "세션을 찾을 수 없습니다.", "불러오기 실패",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    RefreshSessionList();
                    return;
                }

                if (_playback != null) _playback.Pause();
                btnPlayPause.Text = "▶ 재생";

                if (data.HeatmapGrid != null) _heatmap.LoadGridData(data.HeatmapGrid); else _heatmap.Reset();
                _chart.LoadFromLogs(data.ChartLogRoi, data.ChartLogBg);
                RefreshHeatmapDisplay();
                SetStatus("세션을 불러왔습니다 (히트맵/그래프 복원): " + data.Name);

                if (!string.IsNullOrEmpty(data.VideoPath) && !File.Exists(data.VideoPath))
                {
                    MessageBox.Show(this,
                        "히트맵과 그래프를 복원했습니다. 원본 영상 경로를 찾을 수 없어 재생은 불가합니다:\n" + data.VideoPath,
                        "참고", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (SessionPersistenceException spex)
            {
                MessageBox.Show(this, spex.Message, "불러오기 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) { ShowError("세션을 불러오는 중 오류가 발생했습니다.", ex); }
        }

        private void btnDeleteSession_Click(object sender, EventArgs e)
        {
            string id = GetSelectedSessionId();
            if (id == null) { WarnSelect("삭제"); return; }

            if (MessageBox.Show(this, "선택한 세션을 삭제하시겠습니까? (JSON + PNG)", "삭제 확인",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                _sessions.Delete(id);
                RefreshSessionList();
                SetStatus("세션을 삭제했습니다.");
            }
            catch (SessionPersistenceException spex)
            {
                MessageBox.Show(this, spex.Message, "삭제 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) { ShowError("세션 삭제 중 오류가 발생했습니다.", ex); }
        }

        private void btnRefreshSessions_Click(object sender, EventArgs e)
        {
            RefreshSessionList();
            SetStatus("세션 목록을 새로고침했습니다.");
        }

        private void listViewSessions_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlState();
        }

        private SessionData BuildSessionData(string name, string memo)
        {
            SessionData data = new SessionData();
            data.Name = name;
            data.Memo = memo;
            data.VideoPath = _video != null ? _video.VideoPath : string.Empty;
            data.HeatmapGrid = _heatmap.ExportGridData();
            data.ChartLogRoi = _chart.GetLogA();
            data.ChartLogBg = _chart.GetLogB();
            return data;
        }

        private void RefreshSessionList()
        {
            if (_sessions == null) return;
            try
            {
                List<SessionSummary> all = _sessions.ReadAll();
                listViewSessions.BeginUpdate();
                listViewSessions.Items.Clear();
                for (int i = 0; i < all.Count; i++)
                {
                    SessionSummary s = all[i];
                    ListViewItem item = new ListViewItem(string.IsNullOrEmpty(s.Name) ? "(이름 없음)" : s.Name);
                    item.SubItems.Add(string.IsNullOrEmpty(s.VideoPath) ? "(경로 없음)" : s.VideoPath);
                    item.SubItems.Add(s.CreatedAt.ToString("yyyy-MM-dd HH:mm"));
                    item.SubItems.Add(s.UpdatedAt.ToString("yyyy-MM-dd HH:mm"));
                    item.ToolTipText = s.VideoPath;   // full path on hover
                    item.Tag = s.SessionId;
                    listViewSessions.Items.Add(item);
                }
                listViewSessions.EndUpdate();
                listViewSessions.ShowItemToolTips = true;
                UpdateControlState();
            }
            catch (SessionPersistenceException spex)
            {
                MessageBox.Show(this, spex.Message, "목록 로드 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) { ShowError("세션 목록을 불러오는 중 오류가 발생했습니다.", ex); }
        }

        private string GetSelectedSessionId()
        {
            if (listViewSessions.SelectedItems.Count == 0) return null;
            object tag = listViewSessions.SelectedItems[0].Tag;
            return tag == null ? null : tag.ToString();
        }

        // ----------------------------------------------------------------- helpers

        private bool IsVideoReady()
        {
            return _video != null && _video.IsLoaded && _playback != null;
        }

        private void UpdateControlState()
        {
            bool ready = _video != null && _video.IsLoaded;
            btnPlayPause.Enabled = ready;
            btnStop.Enabled = ready;
            btnPrevFrame.Enabled = ready;
            btnNextFrame.Enabled = ready;
            trackBarSeek.Enabled = ready;

            bool hasSelection = listViewSessions.SelectedItems.Count > 0;
            btnUpdateSession.Enabled = hasSelection;
            btnLoadSession.Enabled = hasSelection;
            btnDeleteSession.Enabled = hasSelection;
        }

        private void UpdateFrameInfo(int index)
        {
            int total = _video != null ? _video.TotalFrames : 0;
            lblFrameInfo.Text = string.Format("프레임: {0} / {1}", index, total > 0 ? total - 1 : 0);
        }

        private void UpdateSeekBar(int index)
        {
            if (_video == null || _video.TotalFrames <= 1) return;
            int pct = (int)((long)index * 100 / (_video.TotalFrames - 1));
            if (pct < 0) pct = 0; if (pct > 100) pct = 100;
            _suppressSeekEvent = true; trackBarSeek.Value = pct; _suppressSeekEvent = false;
        }

        private void WarnSelect(string action)
        {
            MessageBox.Show(this, action + " 세션을 목록에서 선택하세요.", "선택 필요",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SetStatus(string text) { lblStatus.Text = text; }

        private void ShowError(string context, Exception ex)
        {
            MessageBox.Show(this, context + "\n\n" + ex.Message, "오류",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            SetStatus("오류: " + ex.Message);
        }

        private static void SetPictureImage(PictureBox box, Image image)
        {
            Image old = box.Image;
            box.Image = image;
            if (old != null && !ReferenceEquals(old, image)) old.Dispose();
        }

        private static void DisposePictureImage(PictureBox box)
        {
            if (box != null && box.Image != null)
            {
                Image old = box.Image; box.Image = null; old.Dispose();
            }
        }

        private static void DisposeBitmap(ref Bitmap bmp)
        {
            if (bmp != null) { bmp.Dispose(); bmp = null; }
        }
    }
}
