using System;
using System.Collections.Generic;
using System.Text;

namespace BSE.Tunes.XApp
{
    public class PlayStateChangedEventArgs : EventArgs
    {
        public PlayStateChangedEventArgs(AudioPlayerState oldState, AudioPlayerState newState)
        {
            OldState = oldState;
            NewState = newState;
        }

        public AudioPlayerState OldState { get; }
        public AudioPlayerState NewState { get; }
    }
}
