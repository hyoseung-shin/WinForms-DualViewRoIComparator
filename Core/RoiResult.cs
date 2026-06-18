using System.Drawing;

namespace DualViewRoiComparator.Core
{
    /// <summary>
    /// Result of analyzing a single video frame against its predecessor.
    /// Produced by an <see cref="IRoiBackend"/> implementation and consumed by the UI,
    /// the heatmap accumulator and the chart manager.
    /// </summary>
    public sealed class RoiResult
    {
        /// <summary>
        /// Bounding boxes (in processing-resolution coordinates) of detected
        /// motion regions of interest.
        /// </summary>
        public Rectangle[] BoundingBoxes { get; set; }

        /// <summary>
        /// Binary motion mask at processing resolution, indexed [y, x].
        /// True where motion was detected.
        /// </summary>
        public bool[,] MotionMask { get; set; }

        /// <summary>Approximate (heuristic) bit cost attributed to the ROI region.</summary>
        public double EstimatedBitsRoi { get; set; }

        /// <summary>Approximate (heuristic) bit cost attributed to the background region.</summary>
        public double EstimatedBitsBackground { get; set; }

        public RoiResult()
        {
            BoundingBoxes = new Rectangle[0];
        }

        /// <summary>
        /// Builds an empty result (used for the very first frame, which has no predecessor).
        /// </summary>
        public static RoiResult Empty(int width, int height)
        {
            return new RoiResult
            {
                BoundingBoxes = new Rectangle[0],
                MotionMask = new bool[height, width],
                EstimatedBitsRoi = 0.0,
                EstimatedBitsBackground = 0.0
            };
        }
    }
}
