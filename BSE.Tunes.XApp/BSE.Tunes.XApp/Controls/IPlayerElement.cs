using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace BSE.Tunes.XApp.Controls
{
    interface IPlayerElement
    {
        object PlayCommandParameter { get; set; }
        ICommand PlayCommand { get; set; }

        object PauseCommandParameter { get; set; }
        ICommand PauseCommand { get; set; }

        object PlayNextCommandParameter { get; set; }
        ICommand PlayNextCommand { get; set; }

        object Progress { get; set; }
    }
}
