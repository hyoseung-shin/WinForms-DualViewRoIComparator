using System;
using System.Collections.Generic;

namespace DualViewRoiComparator.Persistence
{
    /// <summary>A single estimated-bits sample for the comparison chart.</summary>
    public sealed class BitPoint
    {
        public int Frame { get; set; }
        public double Bits { get; set; }

        public BitPoint() { }

        public BitPoint(int frame, double bits)
        {
            Frame = frame;
            Bits = bits;
        }
    }

    /// <summary>
    /// Full persisted analysis session (single-video). Serialised to
    /// <c>/Sessions/{SessionId}.json</c>; the heatmap is also exported as
    /// <c>/Sessions/{SessionId}.png</c> for visual verification.
    /// </summary>
    public sealed class SessionData
    {
        public string SessionId { get; set; }
        public string Name { get; set; }
        public string Memo { get; set; }
        public string VideoPath { get; set; }          // absolute Windows path of the source video
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int[,] HeatmapGrid { get; set; }        // accumulated spatial-density grid
        public string HeatmapImagePath { get; set; }   // absolute path of the saved .png

        public List<BitPoint> ChartLogRoi { get; set; }   // ROI estimated bits
        public List<BitPoint> ChartLogBg { get; set; }    // background estimated bits

        public SessionData()
        {
            ChartLogRoi = new List<BitPoint>();
            ChartLogBg = new List<BitPoint>();
        }
    }

    /// <summary>
    /// Lightweight session descriptor stored in <c>/Sessions/index.json</c> and shown in the
    /// session list (including the absolute video path and saved heatmap image path).
    /// </summary>
    public sealed class SessionSummary
    {
        public string SessionId { get; set; }
        public string Name { get; set; }
        public string Memo { get; set; }
        public string VideoPath { get; set; }
        public string HeatmapImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public SessionSummary() { }

        public static SessionSummary FromData(SessionData data)
        {
            return new SessionSummary
            {
                SessionId = data.SessionId,
                Name = data.Name,
                Memo = data.Memo,
                VideoPath = data.VideoPath,
                HeatmapImagePath = data.HeatmapImagePath,
                CreatedAt = data.CreatedAt,
                UpdatedAt = data.UpdatedAt
            };
        }
    }
}
