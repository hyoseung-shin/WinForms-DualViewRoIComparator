namespace DualViewRoiComparator
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuOpenVideo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ToolStripMenuItem menuReset;
        private System.Windows.Forms.ToolStripMenuItem menuResetHeatmap;
        private System.Windows.Forms.ToolStripMenuItem menuResetChart;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuHelpInfo;

        private System.Windows.Forms.GroupBox grpOriginal;
        private System.Windows.Forms.PictureBox pictureBoxOriginal;
        private System.Windows.Forms.GroupBox grpRoi;
        private System.Windows.Forms.PictureBox pictureBoxRoi;
        private System.Windows.Forms.GroupBox grpHeatmap;
        private System.Windows.Forms.PictureBox pictureBoxHeatmap;

        private System.Windows.Forms.GroupBox grpPlayback;
        private System.Windows.Forms.Button btnPrevFrame;
        private System.Windows.Forms.Button btnPlayPause;
        private System.Windows.Forms.Button btnNextFrame;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TrackBar trackBarSeek;
        private System.Windows.Forms.Label lblFrameInfo;
        private System.Windows.Forms.Label lblThresholdCaption;
        private System.Windows.Forms.TrackBar trackBarThreshold;
        private System.Windows.Forms.Label lblThreshold;

        private System.Windows.Forms.GroupBox grpChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartBits;

        private System.Windows.Forms.GroupBox grpSessions;
        private System.Windows.Forms.ListView listViewSessions;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colPath;
        private System.Windows.Forms.ColumnHeader colCreated;
        private System.Windows.Forms.ColumnHeader colUpdated;
        private System.Windows.Forms.Button btnSaveSession;
        private System.Windows.Forms.Button btnUpdateSession;
        private System.Windows.Forms.Button btnLoadSession;
        private System.Windows.Forms.Button btnDeleteSession;
        private System.Windows.Forms.Button btnRefreshSessions;

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblBackend;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 =
                new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpenVideo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuReset = new System.Windows.Forms.ToolStripMenuItem();
            this.menuResetHeatmap = new System.Windows.Forms.ToolStripMenuItem();
            this.menuResetChart = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.grpOriginal = new System.Windows.Forms.GroupBox();
            this.pictureBoxOriginal = new System.Windows.Forms.PictureBox();
            this.grpRoi = new System.Windows.Forms.GroupBox();
            this.pictureBoxRoi = new System.Windows.Forms.PictureBox();
            this.grpHeatmap = new System.Windows.Forms.GroupBox();
            this.pictureBoxHeatmap = new System.Windows.Forms.PictureBox();
            this.grpPlayback = new System.Windows.Forms.GroupBox();
            this.btnPrevFrame = new System.Windows.Forms.Button();
            this.btnPlayPause = new System.Windows.Forms.Button();
            this.btnNextFrame = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.trackBarSeek = new System.Windows.Forms.TrackBar();
            this.lblFrameInfo = new System.Windows.Forms.Label();
            this.lblThresholdCaption = new System.Windows.Forms.Label();
            this.trackBarThreshold = new System.Windows.Forms.TrackBar();
            this.lblThreshold = new System.Windows.Forms.Label();
            this.grpChart = new System.Windows.Forms.GroupBox();
            this.chartBits = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.grpSessions = new System.Windows.Forms.GroupBox();
            this.listViewSessions = new System.Windows.Forms.ListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colPath = new System.Windows.Forms.ColumnHeader();
            this.colCreated = new System.Windows.Forms.ColumnHeader();
            this.colUpdated = new System.Windows.Forms.ColumnHeader();
            this.btnSaveSession = new System.Windows.Forms.Button();
            this.btnUpdateSession = new System.Windows.Forms.Button();
            this.btnLoadSession = new System.Windows.Forms.Button();
            this.btnDeleteSession = new System.Windows.Forms.Button();
            this.btnRefreshSessions = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblBackend = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            this.grpOriginal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginal)).BeginInit();
            this.grpRoi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRoi)).BeginInit();
            this.grpHeatmap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHeatmap)).BeginInit();
            this.grpPlayback.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSeek)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThreshold)).BeginInit();
            this.grpChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartBits)).BeginInit();
            this.grpSessions.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            //
            // menuStrip1
            //
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuFile,
                this.menuReset,
                this.menuHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1192, 24);
            this.menuStrip1.TabIndex = 0;
            //
            // menuFile
            //
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuOpenVideo,
                this.toolStripSeparator1,
                this.menuExit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(57, 20);
            this.menuFile.Text = "파일(&F)";
            //
            // menuOpenVideo
            //
            this.menuOpenVideo.Name = "menuOpenVideo";
            this.menuOpenVideo.Size = new System.Drawing.Size(180, 22);
            this.menuOpenVideo.Text = "영상 열기(&O)";
            this.menuOpenVideo.Click += new System.EventHandler(this.menuOpenVideo_Click);
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            //
            // menuExit
            //
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(180, 22);
            this.menuExit.Text = "종료(&X)";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            //
            // menuReset
            //
            this.menuReset.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuResetHeatmap,
                this.menuResetChart});
            this.menuReset.Name = "menuReset";
            this.menuReset.Size = new System.Drawing.Size(67, 20);
            this.menuReset.Text = "초기화(&R)";
            //
            // menuResetHeatmap
            //
            this.menuResetHeatmap.Name = "menuResetHeatmap";
            this.menuResetHeatmap.Size = new System.Drawing.Size(180, 22);
            this.menuResetHeatmap.Text = "히트맵 초기화";
            this.menuResetHeatmap.Click += new System.EventHandler(this.menuResetHeatmap_Click);
            //
            // menuResetChart
            //
            this.menuResetChart.Name = "menuResetChart";
            this.menuResetChart.Size = new System.Drawing.Size(180, 22);
            this.menuResetChart.Text = "그래프 초기화";
            this.menuResetChart.Click += new System.EventHandler(this.menuResetChart_Click);
            //
            // menuHelp
            //
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuHelpInfo});
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(73, 20);
            this.menuHelp.Text = "도움말(&H)";
            //
            // menuHelpInfo
            //
            this.menuHelpInfo.Name = "menuHelpInfo";
            this.menuHelpInfo.Size = new System.Drawing.Size(200, 22);
            this.menuHelpInfo.Text = "파일 업로드 안내 및 주의사항";
            this.menuHelpInfo.Click += new System.EventHandler(this.menuHelpInfo_Click);
            //
            // grpOriginal
            //
            this.grpOriginal.Controls.Add(this.pictureBoxOriginal);
            this.grpOriginal.Location = new System.Drawing.Point(8, 30);
            this.grpOriginal.Name = "grpOriginal";
            this.grpOriginal.Size = new System.Drawing.Size(388, 300);
            this.grpOriginal.TabIndex = 1;
            this.grpOriginal.TabStop = false;
            this.grpOriginal.Text = "① 원본 영상";
            //
            // pictureBoxOriginal
            //
            this.pictureBoxOriginal.BackColor = System.Drawing.Color.Black;
            this.pictureBoxOriginal.Location = new System.Drawing.Point(8, 20);
            this.pictureBoxOriginal.Name = "pictureBoxOriginal";
            this.pictureBoxOriginal.Size = new System.Drawing.Size(372, 272);
            this.pictureBoxOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxOriginal.TabStop = false;
            //
            // grpRoi
            //
            this.grpRoi.Controls.Add(this.pictureBoxRoi);
            this.grpRoi.Location = new System.Drawing.Point(400, 30);
            this.grpRoi.Name = "grpRoi";
            this.grpRoi.Size = new System.Drawing.Size(388, 300);
            this.grpRoi.TabIndex = 2;
            this.grpRoi.TabStop = false;
            this.grpRoi.Text = "② ROI 적용 영상";
            //
            // pictureBoxRoi
            //
            this.pictureBoxRoi.BackColor = System.Drawing.Color.Black;
            this.pictureBoxRoi.Location = new System.Drawing.Point(8, 20);
            this.pictureBoxRoi.Name = "pictureBoxRoi";
            this.pictureBoxRoi.Size = new System.Drawing.Size(372, 272);
            this.pictureBoxRoi.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxRoi.TabStop = false;
            //
            // grpHeatmap
            //
            this.grpHeatmap.Controls.Add(this.pictureBoxHeatmap);
            this.grpHeatmap.Location = new System.Drawing.Point(792, 30);
            this.grpHeatmap.Name = "grpHeatmap";
            this.grpHeatmap.Size = new System.Drawing.Size(392, 300);
            this.grpHeatmap.TabIndex = 3;
            this.grpHeatmap.TabStop = false;
            this.grpHeatmap.Text = "③ 공간 밀도 히트맵";
            //
            // pictureBoxHeatmap
            //
            this.pictureBoxHeatmap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.pictureBoxHeatmap.Location = new System.Drawing.Point(8, 20);
            this.pictureBoxHeatmap.Name = "pictureBoxHeatmap";
            this.pictureBoxHeatmap.Size = new System.Drawing.Size(376, 272);
            this.pictureBoxHeatmap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxHeatmap.TabStop = false;
            //
            // grpPlayback
            //
            this.grpPlayback.Controls.Add(this.btnPrevFrame);
            this.grpPlayback.Controls.Add(this.btnPlayPause);
            this.grpPlayback.Controls.Add(this.btnNextFrame);
            this.grpPlayback.Controls.Add(this.btnStop);
            this.grpPlayback.Controls.Add(this.trackBarSeek);
            this.grpPlayback.Controls.Add(this.lblFrameInfo);
            this.grpPlayback.Controls.Add(this.lblThresholdCaption);
            this.grpPlayback.Controls.Add(this.trackBarThreshold);
            this.grpPlayback.Controls.Add(this.lblThreshold);
            this.grpPlayback.Location = new System.Drawing.Point(8, 336);
            this.grpPlayback.Name = "grpPlayback";
            this.grpPlayback.Size = new System.Drawing.Size(1176, 96);
            this.grpPlayback.TabIndex = 4;
            this.grpPlayback.TabStop = false;
            this.grpPlayback.Text = "재생 제어";
            //
            // btnPrevFrame
            //
            this.btnPrevFrame.Location = new System.Drawing.Point(12, 22);
            this.btnPrevFrame.Name = "btnPrevFrame";
            this.btnPrevFrame.Size = new System.Drawing.Size(60, 30);
            this.btnPrevFrame.TabIndex = 0;
            this.btnPrevFrame.Text = "◀ 이전";
            this.btnPrevFrame.UseVisualStyleBackColor = true;
            this.btnPrevFrame.Click += new System.EventHandler(this.btnPrevFrame_Click);
            //
            // btnPlayPause
            //
            this.btnPlayPause.Location = new System.Drawing.Point(78, 22);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(80, 30);
            this.btnPlayPause.TabIndex = 1;
            this.btnPlayPause.Text = "▶ 재생";
            this.btnPlayPause.UseVisualStyleBackColor = true;
            this.btnPlayPause.Click += new System.EventHandler(this.btnPlayPause_Click);
            //
            // btnNextFrame
            //
            this.btnNextFrame.Location = new System.Drawing.Point(164, 22);
            this.btnNextFrame.Name = "btnNextFrame";
            this.btnNextFrame.Size = new System.Drawing.Size(60, 30);
            this.btnNextFrame.TabIndex = 2;
            this.btnNextFrame.Text = "다음 ▶";
            this.btnNextFrame.UseVisualStyleBackColor = true;
            this.btnNextFrame.Click += new System.EventHandler(this.btnNextFrame_Click);
            //
            // btnStop
            //
            this.btnStop.Location = new System.Drawing.Point(230, 22);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(60, 30);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "■ 정지";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            //
            // trackBarSeek
            //
            this.trackBarSeek.Location = new System.Drawing.Point(300, 20);
            this.trackBarSeek.Maximum = 100;
            this.trackBarSeek.Name = "trackBarSeek";
            this.trackBarSeek.Size = new System.Drawing.Size(620, 45);
            this.trackBarSeek.TabIndex = 4;
            this.trackBarSeek.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarSeek.Scroll += new System.EventHandler(this.trackBarSeek_Scroll);
            //
            // lblFrameInfo
            //
            this.lblFrameInfo.AutoSize = true;
            this.lblFrameInfo.Location = new System.Drawing.Point(936, 28);
            this.lblFrameInfo.Name = "lblFrameInfo";
            this.lblFrameInfo.Size = new System.Drawing.Size(89, 12);
            this.lblFrameInfo.TabIndex = 5;
            this.lblFrameInfo.Text = "프레임: - / -";
            //
            // lblThresholdCaption
            //
            this.lblThresholdCaption.AutoSize = true;
            this.lblThresholdCaption.Location = new System.Drawing.Point(12, 66);
            this.lblThresholdCaption.Name = "lblThresholdCaption";
            this.lblThresholdCaption.Size = new System.Drawing.Size(135, 12);
            this.lblThresholdCaption.TabIndex = 6;
            this.lblThresholdCaption.Text = "임계값 (Threshold 0-255)";
            //
            // trackBarThreshold
            //
            this.trackBarThreshold.Location = new System.Drawing.Point(160, 60);
            this.trackBarThreshold.Maximum = 255;
            this.trackBarThreshold.Name = "trackBarThreshold";
            this.trackBarThreshold.Size = new System.Drawing.Size(240, 45);
            this.trackBarThreshold.TabIndex = 7;
            this.trackBarThreshold.TickFrequency = 16;
            this.trackBarThreshold.Value = 25;
            this.trackBarThreshold.Scroll += new System.EventHandler(this.trackBarThreshold_Scroll);
            //
            // lblThreshold
            //
            this.lblThreshold.AutoSize = true;
            this.lblThreshold.Location = new System.Drawing.Point(410, 66);
            this.lblThreshold.Name = "lblThreshold";
            this.lblThreshold.Size = new System.Drawing.Size(25, 12);
            this.lblThreshold.TabIndex = 8;
            this.lblThreshold.Text = "25";
            //
            // grpChart
            //
            this.grpChart.Controls.Add(this.chartBits);
            this.grpChart.Location = new System.Drawing.Point(8, 438);
            this.grpChart.Name = "grpChart";
            this.grpChart.Size = new System.Drawing.Size(720, 352);
            this.grpChart.TabIndex = 5;
            this.grpChart.TabStop = false;
            this.grpChart.Text = "추정 비트량 그래프 (ROI vs 배경, 근사 모델)";
            //
            // chartBits
            //
            chartArea1.Name = "ChartArea1";
            this.chartBits.ChartAreas.Add(chartArea1);
            this.chartBits.Location = new System.Drawing.Point(8, 20);
            this.chartBits.Name = "chartBits";
            this.chartBits.Size = new System.Drawing.Size(704, 324);
            this.chartBits.TabIndex = 0;
            //
            // grpSessions
            //
            this.grpSessions.Controls.Add(this.listViewSessions);
            this.grpSessions.Controls.Add(this.btnSaveSession);
            this.grpSessions.Controls.Add(this.btnUpdateSession);
            this.grpSessions.Controls.Add(this.btnLoadSession);
            this.grpSessions.Controls.Add(this.btnDeleteSession);
            this.grpSessions.Controls.Add(this.btnRefreshSessions);
            this.grpSessions.Location = new System.Drawing.Point(736, 438);
            this.grpSessions.Name = "grpSessions";
            this.grpSessions.Size = new System.Drawing.Size(448, 352);
            this.grpSessions.TabIndex = 6;
            this.grpSessions.TabStop = false;
            this.grpSessions.Text = "분석 세션 관리 (CRUD)";
            //
            // listViewSessions
            //
            this.listViewSessions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.colName,
                this.colPath,
                this.colCreated,
                this.colUpdated});
            this.listViewSessions.FullRowSelect = true;
            this.listViewSessions.HideSelection = false;
            this.listViewSessions.Location = new System.Drawing.Point(10, 22);
            this.listViewSessions.MultiSelect = false;
            this.listViewSessions.Name = "listViewSessions";
            this.listViewSessions.Size = new System.Drawing.Size(428, 240);
            this.listViewSessions.TabIndex = 0;
            this.listViewSessions.UseCompatibleStateImageBehavior = false;
            this.listViewSessions.View = System.Windows.Forms.View.Details;
            this.listViewSessions.SelectedIndexChanged += new System.EventHandler(this.listViewSessions_SelectedIndexChanged);
            //
            // colName
            //
            this.colName.Text = "이름";
            this.colName.Width = 130;
            //
            // colPath
            //
            this.colPath.Text = "파일 경로 (절대 경로)";
            this.colPath.Width = 260;
            //
            // colCreated
            //
            this.colCreated.Text = "생성";
            this.colCreated.Width = 120;
            //
            // colUpdated
            //
            this.colUpdated.Text = "수정";
            this.colUpdated.Width = 120;
            //
            // btnSaveSession
            //
            this.btnSaveSession.Location = new System.Drawing.Point(10, 270);
            this.btnSaveSession.Name = "btnSaveSession";
            this.btnSaveSession.Size = new System.Drawing.Size(104, 30);
            this.btnSaveSession.TabIndex = 1;
            this.btnSaveSession.Text = "저장 (Create)";
            this.btnSaveSession.UseVisualStyleBackColor = true;
            this.btnSaveSession.Click += new System.EventHandler(this.btnSaveSession_Click);
            //
            // btnUpdateSession
            //
            this.btnUpdateSession.Location = new System.Drawing.Point(120, 270);
            this.btnUpdateSession.Name = "btnUpdateSession";
            this.btnUpdateSession.Size = new System.Drawing.Size(104, 30);
            this.btnUpdateSession.TabIndex = 2;
            this.btnUpdateSession.Text = "수정 (Update)";
            this.btnUpdateSession.UseVisualStyleBackColor = true;
            this.btnUpdateSession.Click += new System.EventHandler(this.btnUpdateSession_Click);
            //
            // btnLoadSession
            //
            this.btnLoadSession.Location = new System.Drawing.Point(230, 270);
            this.btnLoadSession.Name = "btnLoadSession";
            this.btnLoadSession.Size = new System.Drawing.Size(104, 30);
            this.btnLoadSession.TabIndex = 3;
            this.btnLoadSession.Text = "불러오기 (Read)";
            this.btnLoadSession.UseVisualStyleBackColor = true;
            this.btnLoadSession.Click += new System.EventHandler(this.btnLoadSession_Click);
            //
            // btnDeleteSession
            //
            this.btnDeleteSession.Location = new System.Drawing.Point(340, 270);
            this.btnDeleteSession.Name = "btnDeleteSession";
            this.btnDeleteSession.Size = new System.Drawing.Size(98, 30);
            this.btnDeleteSession.TabIndex = 4;
            this.btnDeleteSession.Text = "삭제 (Delete)";
            this.btnDeleteSession.UseVisualStyleBackColor = true;
            this.btnDeleteSession.Click += new System.EventHandler(this.btnDeleteSession_Click);
            //
            // btnRefreshSessions
            //
            this.btnRefreshSessions.Location = new System.Drawing.Point(10, 306);
            this.btnRefreshSessions.Name = "btnRefreshSessions";
            this.btnRefreshSessions.Size = new System.Drawing.Size(160, 30);
            this.btnRefreshSessions.TabIndex = 5;
            this.btnRefreshSessions.Text = "목록 새로고침";
            this.btnRefreshSessions.UseVisualStyleBackColor = true;
            this.btnRefreshSessions.Click += new System.EventHandler(this.btnRefreshSessions_Click);
            //
            // statusStrip1
            //
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.lblStatus,
                this.lblBackend});
            this.statusStrip1.Location = new System.Drawing.Point(0, 798);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1192, 22);
            this.statusStrip1.TabIndex = 7;
            //
            // lblStatus
            //
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(60, 17);
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "준비됨";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblBackend
            //
            this.lblBackend.Name = "lblBackend";
            this.lblBackend.Size = new System.Drawing.Size(60, 17);
            this.lblBackend.Text = "Backend: -";
            //
            // openFileDialog1
            //
            this.openFileDialog1.Filter = "비디오/RAW 파일|*.mp4;*.avi;*.mov;*.mkv;*.wmv;*.yuv|raw YUV(*.yuv)|*.yuv|모든 파일|*.*";
            this.openFileDialog1.Title = "영상 파일 선택";
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1192, 820);
            this.Controls.Add(this.grpSessions);
            this.Controls.Add(this.grpChart);
            this.Controls.Add(this.grpPlayback);
            this.Controls.Add(this.grpHeatmap);
            this.Controls.Add(this.grpRoi);
            this.Controls.Add(this.grpOriginal);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Single-View ROI Comparator with Spatial Density Heatmap";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.grpOriginal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginal)).EndInit();
            this.grpRoi.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRoi)).EndInit();
            this.grpHeatmap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHeatmap)).EndInit();
            this.grpPlayback.ResumeLayout(false);
            this.grpPlayback.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSeek)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThreshold)).EndInit();
            this.grpChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartBits)).EndInit();
            this.grpSessions.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
