using System;
using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace DualViewRoiComparator.Core
{
    /// <summary>
    /// OpenCvSharp-accelerated ROI backend. Performs grayscale conversion, blurring,
    /// absolute differencing and binarization with native OpenCV routines, then defers
    /// bounding-box extraction and bit estimation to the shared <see cref="RoiPostProcessor"/>.
    /// </summary>
    public sealed class OpenCvRoiBackend : IRoiBackend
    {
        public string Name { get { return "OpenCvSharp (native)"; } }

        public RoiResult Analyze(Bitmap previous, Bitmap current, int threshold, double bitWeight, int minBoxArea)
        {
            if (current == null) throw new ArgumentNullException("current");

            int width = current.Width;
            int height = current.Height;

            using (Mat currentMat = BitmapConverter.ToMat(current))
            using (Mat grayCurrentMat = new Mat())
            {
                ToGray(currentMat, grayCurrentMat);

                byte[,] grayCurrent = ToByteArray(grayCurrentMat, width, height);

                if (previous == null)
                {
                    return RoiPostProcessor.Build(grayCurrent, new bool[height, width], bitWeight, minBoxArea);
                }

                using (Mat previousMat = BitmapConverter.ToMat(previous))
                using (Mat grayPreviousMat = new Mat())
                using (Mat blurCurrent = new Mat())
                using (Mat blurPrevious = new Mat())
                using (Mat diff = new Mat())
                using (Mat binary = new Mat())
                {
                    ToGray(previousMat, grayPreviousMat);

                    // Light blur suppresses single-pixel sensor noise before differencing.
                    Cv2.GaussianBlur(grayCurrentMat, blurCurrent, new OpenCvSharp.Size(3, 3), 0);
                    Cv2.GaussianBlur(grayPreviousMat, blurPrevious, new OpenCvSharp.Size(3, 3), 0);

                    Cv2.Absdiff(blurCurrent, blurPrevious, diff);
                    Cv2.Threshold(diff, binary, threshold, 255, ThresholdTypes.Binary);
                    Cv2.Dilate(binary, binary, null, null, 1);

                    bool[,] mask = ToBoolArray(binary, width, height);
                    return RoiPostProcessor.Build(grayCurrent, mask, bitWeight, minBoxArea);
                }
            }
        }

        private static void ToGray(Mat source, Mat destination)
        {
            int channels = source.Channels();
            if (channels == 1)
            {
                source.CopyTo(destination);
            }
            else if (channels == 4)
            {
                Cv2.CvtColor(source, destination, ColorConversionCodes.BGRA2GRAY);
            }
            else
            {
                Cv2.CvtColor(source, destination, ColorConversionCodes.BGR2GRAY);
            }
        }

        private static byte[,] ToByteArray(Mat singleChannel, int width, int height)
        {
            var result = new byte[height, width];
            // Mat is contiguous after the conversions above; index per pixel.
            var indexer = singleChannel.GetGenericIndexer<byte>();
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    result[y, x] = indexer[y, x];
            return result;
        }

        private static bool[,] ToBoolArray(Mat binary, int width, int height)
        {
            var result = new bool[height, width];
            var indexer = binary.GetGenericIndexer<byte>();
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    result[y, x] = indexer[y, x] > 0;
            return result;
        }
    }
}
