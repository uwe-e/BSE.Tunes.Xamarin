using System;
using System.Collections.Generic;
using System.Text;

namespace BSE.Tunes.XApp
{
    public class PlayStateChangedEventArgs : EventArgs
    {
        public PlayStateChangedEventArgs(PlayState oldState, PlayState newState)
        {
            OldState = oldState;
            NewState = newState;
        }

        public PlayState OldState { get; }
        public PlayState NewState { get; }
    }
}
