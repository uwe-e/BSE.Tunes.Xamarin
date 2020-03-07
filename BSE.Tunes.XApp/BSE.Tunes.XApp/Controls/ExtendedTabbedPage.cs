using System;
using Xamarin.Forms;

namespace BSE.Tunes.XApp.Controls
{
    public class ExtendedTabbedPage : TabbedPage, IPlayerController
    {
        public EventHandler<PlayStateChangedEventArgs> PlayStateChanged;

        PlayState CurrentPlayState { get; set; } = PlayState.Stopped;

        public void SendPauseClicked()
        {
            PlayStateChanged?.Invoke(this, new PlayStateChangedEventArgs(PlayState.Playing, PlayState.Paused));
        }

        public void SendPlayClicked()
        {
            PlayStateChanged?.Invoke(this, new PlayStateChangedEventArgs(CurrentPlayState, PlayState.Playing));
        }
    }
}
