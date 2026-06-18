using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DualViewRoiComparator.Core
{
    /// <summary>
    /// Pure-managed ROI backend. Uses <see cref="BitmapData"/> / LockBits to compute a
    /// grayscale frame difference without any native dependency, so it works even when the
    /// OpenCvSharp native runtime is unavailable. This is the safe fallback referenced in
    /// the specification (section 8: OpenCvSharp load failure).
    /// </summary>
    public sealed class BitmapRoiBackend : IRoiBackend
    {
        public string Name { get { return "Managed Bitmap (LockBits)"; } }

        public RoiResult Analyze(Bitmap previous, Bitmap current, int threshold, double bitWeight, int minBoxArea)
        {
            if (current == null) throw new ArgumentNullException("current");

            int width = current.Width;
            int height = current.Height;

            byte[,] grayCurrent = ToGrayArray(current);

            // No predecessor -> no motion. Still report background bits for this frame.
            if (previous == null)
            {
                return RoiPostProcessor.Build(grayCurrent, new bool[height, width], bitWeight, minBoxArea);
            }

            byte[,] grayPrevious = ToGrayArray(previous);
            var mask = new bool[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int diff = grayCurrent[y, x] - grayPrevious[y, x];
                    if (diff < 0) diff = -diff;
                    mask[y, x] = diff >= threshold;
                }
            }

            return RoiPostProcessor.Build(grayCurrent, mask, bitWeight, minBoxArea);
        }

        /// <summary>
        /// Converts a 24/32-bpp bitmap into an 8-bit luminance array using a single LockBits pass.
        /// </summary>
        private static byte[,] ToGrayArray(Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            var gray = new byte[height, width];

            BitmapData data = bmp.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            try
            {
                int stride = data.Stride;
                int byteCount = stride * height;
                byte[] buffer = new byte[byteCount];
                Marshal.Copy(data.Scan0, buffer, 0, byteCount);

                for (int y = 0; y < height; y++)
                {
                    int row = y * stride;
                    for (int x = 0; x < width; x++)
                    {
                        int idx = row + x * 3;        // 24bpp BGR
                        byte b = buffer[idx];
                        byte g = buffer[idx + 1];
                        byte r = buffer[idx + 2];
                        // ITU-R BT.601 luma approximation in integer arithmetic.
                        gray[y, x] = (byte)((r * 77 + g * 150 + b * 29) >> 8);
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(data);
            }

            return gray;
        }
    }
}
