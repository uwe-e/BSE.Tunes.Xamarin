namespace BSE.Tunes.XApp.Controls
{
    public interface IPlayerController
    {
        void SendPlayClicked();
        void SendPauseClicked();
        void SendPlayNextClicked();
        void SendPlayPreviousClicked();
    }

}
