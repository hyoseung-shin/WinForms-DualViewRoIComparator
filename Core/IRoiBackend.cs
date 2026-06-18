using System.Drawing;

namespace DualViewRoiComparator.Core
{
    /// <summary>
    /// Abstraction over the ROI-detection algorithm. Implementations turn a pair of
    /// consecutive frames into a <see cref="RoiResult"/>. This allows the analysis engine
    /// to be swapped (OpenCvSharp-accelerated vs. pure managed Bitmap) without touching
    /// the rest of the application, satisfying the extensibility requirement.
    /// </summary>
    public interface IRoiBackend
    {
        /// <summary>Human-readable backend name, shown in the status bar.</summary>
        string Name { get; }

        /// <summary>
        /// Analyzes <paramref name="current"/> against <paramref name="previous"/>.
        /// </summary>
        /// <param name="previous">The previous frame, or null for the first frame.</param>
        /// <param name="current">The current frame (never null).</param>
        /// <param name="threshold">Binarization threshold (0-255) for the absolute difference.</param>
        /// <param name="bitWeight">Multiplier converting pixel variance into an estimated bit count.</param>
        /// <param name="minBoxArea">Minimum connected-component area (in pixels) kept as a bounding box.</param>
        RoiResult Analyze(Bitmap previous, Bitmap current, int threshold, double bitWeight, int minBoxArea);
    }
}
