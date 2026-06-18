using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using DualViewRoiComparator.Persistence;

namespace DualViewRoiComparator.UI
{
    /// <summary>
    /// Manages the estimated-bits comparison chart. Maintains a full log of points (for
    /// session persistence) while showing only a sliding window of the most recent samples
    /// on screen, so live updates stay fast regardless of clip length.
    /// </summary>
    public sealed class ChartManager
    {
        private const string SeriesA = "VideoA_ROI";
        private const string SeriesB = "VideoB_ROI";
        private const int VisibleWindow = 300;

        private readonly Chart _chart;
        private readonly List<BitPoint> _logA = new List<BitPoint>();
        private readonly List<BitPoint> _logB = new List<BitPoint>();

        public ChartManager(Chart chart)
        {
            if (chart == null) throw new ArgumentNullException("chart");
            _chart = chart;
            InitializeChart();
        }

        private void InitializeChart()
        {
            _chart.Series.Clear();
            _chart.ChartAreas.Clear();
            _chart.Legends.Clear();

            var area = new ChartArea("MainArea");
            area.AxisX.Title = "프레임 (Frame)";
            area.AxisY.Title = "추정 비트량 (Estimated Bits)";
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            _chart.ChartAreas.Add(area);

            var legend = new Legend("MainLegend");
            legend.Docking = Docking.Top;
            _chart.Legends.Add(legend);

            var seriesA = new Series(SeriesA)
            {
                ChartType = SeriesChartType.Line,
                ChartArea = "MainArea",
                Legend = "MainLegend",
                LegendText = "ROI 추정 비트",
                BorderWidth = 2,
                Color = Color.OrangeRed,
                XValueType = ChartValueType.Int32
            };
            var seriesB = new Series(SeriesB)
            {
                ChartType = SeriesChartType.Line,
                ChartArea = "MainArea",
                Legend = "MainLegend",
                LegendText = "배경 추정 비트",
                BorderWidth = 2,
                Color = Color.DodgerBlue,
                XValueType = ChartValueType.Int32
            };
            _chart.Series.Add(seriesA);
            _chart.Series.Add(seriesB);
        }

        /// <summary>Adds one comparison sample and refreshes the visible window.</summary>
        public void AddPoint(int frame, double bitsA, double bitsB)
        {
            _logA.Add(new BitPoint(frame, bitsA));
            _logB.Add(new BitPoint(frame, bitsB));

            AppendVisible(SeriesA, frame, bitsA);
            AppendVisible(SeriesB, frame, bitsB);
        }

        private void AppendVisible(string seriesName, int frame, double bits)
        {
            Series series = _chart.Series[seriesName];
            series.Points.AddXY(frame, bits);
            while (series.Points.Count > VisibleWindow)
                series.Points.RemoveAt(0);
        }

        public void Reset()
        {
            _logA.Clear();
            _logB.Clear();
            _chart.Series[SeriesA].Points.Clear();
            _chart.Series[SeriesB].Points.Clear();
        }

        public List<BitPoint> GetLogA()
        {
            return new List<BitPoint>(_logA);
        }

        public List<BitPoint> GetLogB()
        {
            return new List<BitPoint>(_logB);
        }

        /// <summary>
        /// Restores chart contents from persisted logs (used when loading a session).
        /// </summary>
        public void LoadFromLogs(List<BitPoint> logA, List<BitPoint> logB)
        {
            Reset();

            if (logA != null)
            {
                foreach (var p in logA)
                {
                    _logA.Add(new BitPoint(p.Frame, p.Bits));
                    AppendVisible(SeriesA, p.Frame, p.Bits);
                }
            }
            if (logB != null)
            {
                foreach (var p in logB)
                {
                    _logB.Add(new BitPoint(p.Frame, p.Bits));
                    AppendVisible(SeriesB, p.Frame, p.Bits);
                }
            }
        }
    }
}
