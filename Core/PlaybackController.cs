using System;
using System.Windows.Forms;

namespace DualViewRoiComparator.Core
{
    /// <summary>
    /// Timer-based playback driver. A single <see cref="System.Windows.Forms.Timer"/> raises
    /// the <see cref="Tick"/> event on the UI thread; the form's handler then reads, analyses
    /// and renders both frames within that one tick, guaranteeing A/B synchronisation without
    /// any multithreading.
    /// </summary>
    public sealed class PlaybackController : IDisposable
    {
        private readonly Timer _timer;

        /// <summary>Raised once per playback tick while playing.</summary>
        public event EventHandler Tick;

        public PlaybackController()
        {
            _timer = new Timer();
            _timer.Interval = 40; // ~25 fps default
            _timer.Tick += OnTimerTick;
        }

        public bool IsPlaying { get { return _timer.Enabled; } }

        /// <summary>Sets the tick interval from a frames-per-second value.</summary>
        public void SetFps(double fps)
        {
            if (fps <= 0 || double.IsNaN(fps)) fps = 25.0;
            int interval = (int)Math.Round(1000.0 / fps);
            if (interval < 10) interval = 10;     // cap at ~100 fps to keep UI responsive
            if (interval > 1000) interval = 1000;
            _timer.Interval = interval;
        }

        public void Play()
        {
            _timer.Start();
        }

        public void Pause()
        {
            _timer.Stop();
        }

        public void Toggle()
        {
            if (IsPlaying) Pause();
            else Play();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            EventHandler handler = Tick;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Tick -= OnTimerTick;
            _timer.Dispose();
        }
    }
}
