using System;
using System.Drawing;

namespace DualViewRoiComparator.Heatmap
{
    /// <summary>
    /// Accumulates per-frame motion masks into a coarse 2D grid (default 64x36) so that the
    /// spatial density of motion can be visualised and persisted cheaply. Each true pixel of
    /// a frame mask increments the grid cell it maps to.
    /// </summary>
    public sealed class HeatmapAccumulator
    {
        private int[,] _grid;

        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }

        public HeatmapAccumulator(int gridWidth = 64, int gridHeight = 36)
        {
            if (gridWidth < 1) gridWidth = 1;
            if (gridHeight < 1) gridHeight = 1;
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            _grid = new int[GridHeight, GridWidth];
        }

        /// <summary>
        /// Adds a motion mask (at processing resolution) into the grid. Each motion pixel is
        /// binned into its corresponding grid cell.
        /// </summary>
        public void Accumulate(bool[,] motionMask)
        {
            if (motionMask == null) return;

            int maskH = motionMask.GetLength(0);
            int maskW = motionMask.GetLength(1);
            if (maskH == 0 || maskW == 0) return;

            for (int y = 0; y < maskH; y++)
            {
                int gy = (int)((long)y * GridHeight / maskH);
                if (gy >= GridHeight) gy = GridHeight - 1;
                for (int x = 0; x < maskW; x++)
                {
                    if (!motionMask[y, x]) continue;
                    int gx = (int)((long)x * GridWidth / maskW);
                    if (gx >= GridWidth) gx = GridWidth - 1;
                    _grid[gy, gx]++;
                }
            }
        }

        public void Reset()
        {
            _grid = new int[GridHeight, GridWidth];
        }

        /// <summary>Returns a rendered colour heatmap of the requested pixel size.</summary>
        public Bitmap GetHeatmapBitmap(int outWidth, int outHeight)
        {
            return ColorMapHelper.Render(_grid, outWidth, outHeight);
        }

        /// <summary>Returns a defensive copy of the accumulation grid for serialization.</summary>
        public int[,] ExportGridData()
        {
            var copy = new int[GridHeight, GridWidth];
            Array.Copy(_grid, copy, _grid.Length);
            return copy;
        }

        /// <summary>
        /// Loads a previously saved grid. The accumulator adopts the dimensions of the
        /// supplied grid so restored sessions render exactly as saved.
        /// </summary>
        public void LoadGridData(int[,] grid)
        {
            if (grid == null)
            {
                Reset();
                return;
            }
            GridHeight = grid.GetLength(0);
            GridWidth = grid.GetLength(1);
            if (GridHeight < 1) GridHeight = 1;
            if (GridWidth < 1) GridWidth = 1;

            _grid = new int[GridHeight, GridWidth];
            Array.Copy(grid, _grid, Math.Min(grid.Length, _grid.Length));
        }
    }
}
