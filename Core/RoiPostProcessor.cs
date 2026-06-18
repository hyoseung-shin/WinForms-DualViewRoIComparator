using System;
using System.Collections.Generic;
using System.Drawing;

namespace DualViewRoiComparator.Core
{
    /// <summary>
    /// Backend-independent post-processing shared by every <see cref="IRoiBackend"/>.
    /// Given a grayscale frame and a binary motion mask, it extracts bounding boxes
    /// (via iterative connected-component labelling) and computes the heuristic
    /// estimated-bit values for the ROI and background regions.
    ///
    /// Centralising this logic here keeps the two backends free of duplicated code:
    /// each backend only has to produce the grayscale array and the binary mask.
    /// </summary>
    internal static class RoiPostProcessor
    {
        public static RoiResult Build(byte[,] grayCurrent, bool[,] motionMask, double bitWeight, int minBoxArea)
        {
            int height = grayCurrent.GetLength(0);
            int width = grayCurrent.GetLength(1);

            Rectangle[] boxes = ExtractBoundingBoxes(motionMask, width, height, minBoxArea);

            // Variance of the current grayscale frame, split into ROI vs. background pixels.
            double roiSum = 0.0, roiSumSq = 0.0;
            double bgSum = 0.0, bgSumSq = 0.0;
            long roiCount = 0, bgCount = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double v = grayCurrent[y, x];
                    if (motionMask[y, x])
                    {
                        roiSum += v;
                        roiSumSq += v * v;
                        roiCount++;
                    }
                    else
                    {
                        bgSum += v;
                        bgSumSq += v * v;
                        bgCount++;
                    }
                }
            }

            double roiVariance = Variance(roiSum, roiSumSq, roiCount);
            double bgVariance = Variance(bgSum, bgSumSq, bgCount);

            // Heuristic, codec-independent approximation: a region's bit cost grows with
            // both its textural variance and the number of pixels it covers.
            double roiPixelFactor = roiCount;
            double bgPixelFactor = bgCount;

            return new RoiResult
            {
                BoundingBoxes = boxes,
                MotionMask = motionMask,
                EstimatedBitsRoi = roiVariance * bitWeight * Math.Sqrt(roiPixelFactor + 1.0),
                EstimatedBitsBackground = bgVariance * bitWeight * Math.Sqrt(bgPixelFactor + 1.0)
            };
        }

        private static double Variance(double sum, double sumSq, long count)
        {
            if (count <= 1) return 0.0;
            double mean = sum / count;
            double variance = (sumSq / count) - (mean * mean);
            return variance < 0.0 ? 0.0 : variance;
        }

        /// <summary>
        /// Labels connected "true" regions of the mask using an explicit-stack flood fill
        /// (4-connectivity) and returns the bounding box of every region whose pixel area
        /// is at least <paramref name="minBoxArea"/>.
        /// </summary>
        private static Rectangle[] ExtractBoundingBoxes(bool[,] mask, int width, int height, int minBoxArea)
        {
            var boxes = new List<Rectangle>();
            var visited = new bool[height, width];
            var stack = new Stack<int>(); // encodes y * width + x

            for (int sy = 0; sy < height; sy++)
            {
                for (int sx = 0; sx < width; sx++)
                {
                    if (!mask[sy, sx] || visited[sy, sx])
                        continue;

                    int minX = sx, maxX = sx, minY = sy, maxY = sy, area = 0;

                    stack.Clear();
                    stack.Push(sy * width + sx);
                    visited[sy, sx] = true;

                    while (stack.Count > 0)
                    {
                        int code = stack.Pop();
                        int cy = code / width;
                        int cx = code % width;
                        area++;

                        if (cx < minX) minX = cx;
                        if (cx > maxX) maxX = cx;
                        if (cy < minY) minY = cy;
                        if (cy > maxY) maxY = cy;

                        PushNeighbour(stack, visited, mask, cx - 1, cy, width, height);
                        PushNeighbour(stack, visited, mask, cx + 1, cy, width, height);
                        PushNeighbour(stack, visited, mask, cx, cy - 1, width, height);
                        PushNeighbour(stack, visited, mask, cx, cy + 1, width, height);
                    }

                    if (area >= minBoxArea)
                    {
                        boxes.Add(new Rectangle(minX, minY, (maxX - minX) + 1, (maxY - minY) + 1));
                    }
                }
            }

            return boxes.ToArray();
        }

        private static void PushNeighbour(Stack<int> stack, bool[,] visited, bool[,] mask,
            int x, int y, int width, int height)
        {
            if (x < 0 || y < 0 || x >= width || y >= height) return;
            if (visited[y, x] || !mask[y, x]) return;
            visited[y, x] = true;
            stack.Push(y * width + x);
        }
    }
}
