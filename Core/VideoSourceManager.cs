using System;
using System.Drawing;
using System.IO;
using OpenCvSharp;
using OpenCvSharp.Extensions;
// Both System.Drawing and OpenCvSharp define a 'Size' type. Bare 'Size' in this file
// always refers to System.Drawing.Size; OpenCvSharp sizes are fully qualified.
using Size = System.Drawing.Size;

namespace DualViewRoiComparator.Core
{
    /// <summary>
    /// Owns a single video source and provides frame-by-frame reading. Container formats
    /// (mp4/avi/...) are read through OpenCV's <see cref="VideoCapture"/>; headerless raw
    /// <c>.yuv</c> files are read through <see cref="RawYuvReader"/> (I420 4:2:0, caller
    /// supplies width/height/fps). Frames are resized to a capped processing resolution so
    /// per-frame analysis stays real-time. Implements <see cref="IDisposable"/>.
    /// </summary>
    public sealed class VideoSourceManager : IDisposable
    {
        private const int MaxProcWidth = 640;
        private const int MaxProcHeight = 360;

        private VideoCapture _cap;     // container formats
        private RawYuvReader _yuv;     // raw .yuv
        private bool _isYuv;

        public bool IsLoaded { get; private set; }
        public int TotalFrames { get; private set; }
        public double Fps { get; private set; }
        public int CurrentIndex { get; private set; }
        public Size ProcessingSize { get; private set; }
        public string VideoPath { get; private set; }

        /// <summary>Loads a container-format video (mp4/avi/mov/mkv/wmv).</summary>
        public bool LoadVideo(string path, out string error)
        {
            error = null;
            Release();
            try
            {
                if (!File.Exists(path)) { error = "영상 파일을 찾을 수 없습니다: " + path; return false; }

                _cap = new VideoCapture(path);
                if (_cap == null || !_cap.IsOpened())
                {
                    error = "영상을 열 수 없습니다. 코덱이 지원되지 않거나 파일이 손상되었을 수 있습니다.";
                    Release();
                    return false;
                }

                int frames = (int)_cap.Get(VideoCaptureProperties.FrameCount);
                TotalFrames = frames > 0 ? frames : int.MaxValue; // some containers report 0

                double fps = _cap.Get(VideoCaptureProperties.Fps);
                Fps = (fps > 0 && !double.IsNaN(fps)) ? fps : 25.0;

                int w = (int)_cap.Get(VideoCaptureProperties.FrameWidth);
                int h = (int)_cap.Get(VideoCaptureProperties.FrameHeight);
                ProcessingSize = ComputeProcessingSize(w, h);

                _isYuv = false;
                VideoPath = path;
                CurrentIndex = -1;
                IsLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                error = "영상 로드 중 오류가 발생했습니다: " + ex.Message;
                Release();
                return false;
            }
        }

        /// <summary>Loads a headerless raw .yuv file (I420 4:2:0) with the given geometry.</summary>
        public bool LoadYuv(string path, int width, int height, double fps, out string error)
        {
            error = null;
            Release();
            try
            {
                if (!File.Exists(path)) { error = "YUV 파일을 찾을 수 없습니다: " + path; return false; }

                _yuv = new RawYuvReader(path, width, height);
                TotalFrames = _yuv.TotalFrames;
                Fps = (fps > 0 && !double.IsNaN(fps)) ? fps : 25.0;
                ProcessingSize = ComputeProcessingSize(width, height);

                _isYuv = true;
                VideoPath = path;
                CurrentIndex = -1;
                IsLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                error = "YUV 로드 중 오류가 발생했습니다: " + ex.Message;
                Release();
                return false;
            }
        }

        /// <summary>
        /// Reads the next frame. Returns false at end-of-stream. Caller owns/disposes the bitmap.
        /// </summary>
        public bool ReadNextFrame(out Bitmap frame, out int index)
        {
            frame = null;
            index = CurrentIndex;
            if (!IsLoaded) return false;
            if (CurrentIndex + 1 >= TotalFrames) return false;

            if (_isYuv)
            {
                Mat bgr = _yuv.ReadFrameMat(CurrentIndex + 1);
                if (bgr == null || bgr.Empty()) { if (bgr != null) bgr.Dispose(); return false; }
                using (bgr) frame = ResizeToProcessing(bgr);
            }
            else
            {
                using (var mat = new Mat())
                {
                    if (!_cap.Read(mat) || mat.Empty()) return false;
                    frame = ResizeToProcessing(mat);
                }
            }

            CurrentIndex++;
            index = CurrentIndex;
            return true;
        }

        /// <summary>Seeks so the next ReadNextFrame() returns <paramref name="frameIndex"/>.</summary>
        public bool Seek(int frameIndex)
        {
            if (!IsLoaded) return false;
            if (frameIndex < 0) frameIndex = 0;
            if (TotalFrames != int.MaxValue && frameIndex >= TotalFrames)
                frameIndex = TotalFrames - 1;

            try
            {
                if (!_isYuv)
                    _cap.Set(VideoCaptureProperties.PosFrames, frameIndex); // raw reader seeks by index
                CurrentIndex = frameIndex - 1;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int GetTotalFrames() { return TotalFrames; }

        private Bitmap ResizeToProcessing(Mat src)
        {
            using (var resized = new Mat())
            {
                Cv2.Resize(src, resized, new OpenCvSharp.Size(ProcessingSize.Width, ProcessingSize.Height));
                return BitmapConverter.ToBitmap(resized);
            }
        }

        private static Size ComputeProcessingSize(int w, int h)
        {
            if (w <= 0) w = MaxProcWidth;
            if (h <= 0) h = MaxProcHeight;
            double scale = Math.Min(Math.Min((double)MaxProcWidth / w, (double)MaxProcHeight / h), 1.0);
            int pw = Math.Max(2, (int)Math.Round(w * scale));
            int ph = Math.Max(2, (int)Math.Round(h * scale));
            return new Size(pw, ph);
        }

        public void Release()
        {
            IsLoaded = false;
            if (_cap != null)
            {
                try { _cap.Release(); } catch { /* ignore */ }
                _cap.Dispose();
                _cap = null;
            }
            if (_yuv != null)
            {
                _yuv.Dispose();
                _yuv = null;
            }
        }

        public void Dispose() { Release(); }
    }
}
