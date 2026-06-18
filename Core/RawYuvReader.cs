using System;
using System.IO;
using OpenCvSharp;

namespace DualViewRoiComparator.Core
{
    /// <summary>
    /// Minimal reader for headerless raw YUV files in 8-bit planar 4:2:0 (I420/YUV420p)
    /// layout — the format commonly used for VVC/VCM test sequences. Because raw YUV has no
    /// container metadata, the caller must supply width/height/fps. Frame count is derived
    /// from the file size. Each frame is decoded to a BGR <see cref="Mat"/> via OpenCV.
    /// </summary>
    public sealed class RawYuvReader : IDisposable
    {
        private FileStream _fs;
        private readonly int _width;
        private readonly int _height;
        private readonly long _frameSize;

        public int TotalFrames { get; private set; }
        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

        public RawYuvReader(string path, int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("YUV 해상도가 올바르지 않습니다.");

            _width = width;
            _height = height;
            _frameSize = (long)width * height * 3 / 2; // Y + U/4 + V/4

            _fs = File.OpenRead(path);
            TotalFrames = _frameSize > 0 ? (int)(_fs.Length / _frameSize) : 0;
            if (TotalFrames <= 0)
                throw new InvalidDataException(
                    "파일 크기가 한 프레임보다 작습니다. 해상도(가로/세로)가 올바른지 확인하세요.");
        }

        /// <summary>
        /// Reads the frame at <paramref name="index"/> and returns it as a new BGR Mat
        /// (caller disposes). Returns null past end-of-stream.
        /// </summary>
        public Mat ReadFrameMat(int index)
        {
            if (_fs == null || index < 0 || index >= TotalFrames) return null;

            long offset = (long)index * _frameSize;
            _fs.Seek(offset, SeekOrigin.Begin);

            byte[] buffer = new byte[_frameSize];
            int total = 0;
            while (total < buffer.Length)
            {
                int n = _fs.Read(buffer, total, buffer.Length - total);
                if (n <= 0) return null; // truncated frame
                total += n;
            }

            // I420 packs all planes into (height * 3/2) rows of a single-channel image.
            using (var yuv = new Mat(_height * 3 / 2, _width, MatType.CV_8UC1, buffer))
            {
                Mat bgr = new Mat();
                Cv2.CvtColor(yuv, bgr, ColorConversionCodes.YUV2BGR_I420);
                return bgr;
            }
        }

        public void Dispose()
        {
            if (_fs != null)
            {
                _fs.Dispose();
                _fs = null;
            }
        }
    }
}
