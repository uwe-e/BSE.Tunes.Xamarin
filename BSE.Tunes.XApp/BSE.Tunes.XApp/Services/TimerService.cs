using BSE.Tunes.XApp.Controls;
using Prism.Events;
using System;

namespace BSE.Tunes.XApp.Services
{
    public class TimerService : ITimerService
    {
        private Timer _timer;
        public event Action TimerElapsed;

        public TimerService()
        {
            _timer = new Timer(TimeSpan.FromSeconds(1), OnTimerCallback);
        }

        private void OnTimerCallback()
        {
            TimerElapsed?.Invoke();
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
