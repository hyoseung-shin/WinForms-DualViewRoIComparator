using System;
using System.Drawing;

namespace DualViewRoiComparator.Heatmap
{
    /// <summary>
    /// Renders a normalised accumulation grid into a Blue -&gt; Green -&gt; Red heatmap bitmap.
    /// The colour mapping is implemented from scratch (no external colormap dependency),
    /// satisfying the specification's "self-implemented gradient" requirement.
    /// </summary>
    public static class ColorMapHelper
    {
        /// <summary>
        /// Produces a bitmap of size <paramref name="outWidth"/> x <paramref name="outHeight"/>
        /// from the accumulation grid. Cells are scaled up with nearest-neighbour blocks.
        /// </summary>
        public static Bitmap Render(int[,] grid, int outWidth, int outHeight)
        {
            if (grid == null) return CreateBlank(outWidth, outHeight);

            int gridH = grid.GetLength(0);
            int gridW = grid.GetLength(1);
            if (gridH == 0 || gridW == 0) return CreateBlank(outWidth, outHeight);
            if (outWidth < 1) outWidth = 1;
            if (outHeight < 1) outHeight = 1;

            int max = 0;
            for (int y = 0; y < gridH; y++)
                for (int x = 0; x < gridW; x++)
                    if (grid[y, x] > max) max = grid[y, x];

            var bmp = new Bitmap(outWidth, outHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(0, 0, 64)); // dark blue background (zero activity)

                if (max <= 0)
                    return bmp;

                double cellW = (double)outWidth / gridW;
                double cellH = (double)outHeight / gridH;

                for (int gy = 0; gy < gridH; gy++)
                {
                    for (int gx = 0; gx < gridW; gx++)
                    {
                        double norm = (double)grid[gy, gx] / max;
                        Color c = MapColor(norm);
                        using (var brush = new SolidBrush(c))
                        {
                            int px = (int)Math.Floor(gx * cellW);
                            int py = (int)Math.Floor(gy * cellH);
                            int pw = (int)Math.Ceiling(cellW) + 1;
                            int ph = (int)Math.Ceiling(cellH) + 1;
                            g.FillRectangle(brush, px, py, pw, ph);
                        }
                    }
                }
            }
            return bmp;
        }

        /// <summary>
        /// Maps a normalised value [0,1] to a Blue -&gt; Green -&gt; Red gradient.
        /// </summary>
        public static Color MapColor(double norm)
        {
            if (norm < 0.0) norm = 0.0;
            if (norm > 1.0) norm = 1.0;

            int r, g, b;
            if (norm < 0.5)
            {
                // Blue -> Green over the first half.
                double t = norm / 0.5;
                r = 0;
                g = (int)(255 * t);
                b = (int)(255 * (1.0 - t));
            }
            else
            {
                // Green -> Red over the second half.
                double t = (norm - 0.5) / 0.5;
                r = (int)(255 * t);
                g = (int)(255 * (1.0 - t));
                b = 0;
            }
            return Color.FromArgb(Clamp(r), Clamp(g), Clamp(b));
        }

        private static int Clamp(int v)
        {
            if (v < 0) return 0;
            if (v > 255) return 255;
            return v;
        }

        private static Bitmap CreateBlank(int w, int h)
        {
            if (w < 1) w = 1;
            if (h < 1) h = 1;
            var bmp = new Bitmap(w, h);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(0, 0, 64));
            }
            return bmp;
        }
    }
}
