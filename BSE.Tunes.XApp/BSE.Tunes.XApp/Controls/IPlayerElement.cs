﻿using System;
using System.Windows.Input;

namespace BSE.Tunes.XApp.Controls
{
    interface IPlayerElement
    {
        object PlayCommandParameter { get; set; }
        ICommand PlayCommand { get; set; }
        void OnPlayCommandCanExecuteChanged(object sender, EventArgs e);

        object PauseCommandParameter { get; set; }
        ICommand PauseCommand { get; set; }
        void OnPauseCommandCanExecuteChanged(object sender, EventArgs e);

        object PlayNextCommandParameter { get; set; }
        ICommand PlayNextCommand { get; set; }
        void OnPlayNextCommandCanExecuteChanged(object sender, EventArgs e);

        double Progress { get; set; }
    }
}
