using System;
using System.Drawing;

namespace DualViewRoiComparator.Core
{
    /// <summary>
    /// High-level façade over an <see cref="IRoiBackend"/>. Holds the user-adjustable
    /// analysis settings (threshold, bit weight, minimum box area) and selects the best
    /// available backend at construction time, falling back to the managed backend when
    /// the OpenCvSharp native runtime cannot be loaded.
    /// </summary>
    public sealed class RoiAnalyzer
    {
        private readonly IRoiBackend _backend;

        /// <summary>Binarization threshold for the frame difference (0-255).</summary>
        public int Threshold { get; set; }

        /// <summary>Multiplier converting variance into estimated bits.</summary>
        public double BitWeight { get; set; }

        /// <summary>Minimum connected-component area (pixels) kept as a ROI box.</summary>
        public int MinBoxArea { get; set; }

        /// <summary>Name of the active backend (for status display).</summary>
        public string BackendName { get { return _backend.Name; } }

        public RoiAnalyzer()
        {
            Threshold = 25;
            BitWeight = 0.05;
            MinBoxArea = 64;
            _backend = SelectBackend();
        }

        public RoiResult Analyze(Bitmap previous, Bitmap current)
        {
            if (current == null) throw new ArgumentNullException("current");
            return _backend.Analyze(previous, current, Threshold, BitWeight, MinBoxArea);
        }

        /// <summary>
        /// Probes the OpenCvSharp native runtime. If a trivial OpenCV operation succeeds,
        /// the accelerated backend is used; otherwise the pure-managed backend is returned.
        /// </summary>
        private static IRoiBackend SelectBackend()
        {
            try
            {
                using (var probe = new OpenCvSharp.Mat(2, 2, OpenCvSharp.MatType.CV_8UC1, OpenCvSharp.Scalar.All(0)))
                {
                    using (var gray = new OpenCvSharp.Mat())
                    {
                        OpenCvSharp.Cv2.Threshold(probe, gray, 1, 255, OpenCvSharp.ThresholdTypes.Binary);
                    }
                }
                return new OpenCvRoiBackend();
            }
            catch (Exception)
            {
                // DllNotFoundException, TypeInitializationException, BadImageFormatException, etc.
                return new BitmapRoiBackend();
            }
        }
    }
}
