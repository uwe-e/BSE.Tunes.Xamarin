using System;
using System.Collections.Generic;
using System.Text;

namespace BSE.Tunes.XApp
{
    public enum PlayState
    {
        Closed = 0,
        Opening = 1,
        Buffering = 2,
        Playing = 3,
        Paused = 4,
        Stopped = 5
    }
}
