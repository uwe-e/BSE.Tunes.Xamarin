using System;
using System.Collections.Generic;
using System.Text;

namespace BSE.Tunes.XApp.Controls
{
    public interface IPlayerController
    {
        void SendPlayClicked();
        void SendPauseClicked();
    }

}
