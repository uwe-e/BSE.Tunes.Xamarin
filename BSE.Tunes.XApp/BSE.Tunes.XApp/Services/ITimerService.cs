using System;

namespace BSE.Tunes.XApp.Services
{
    public interface ITimerService
    {
        event Action TimerElapsed;
        void Start();
        void Stop();
    }
}
