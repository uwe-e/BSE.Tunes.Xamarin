namespace BSE.Tunes.XApp
{
    public enum MediaState
    {
        None = 0,
        Opened = 1,
        Ended = 2,
        NextRequested = 3,
        PreviousRequested = 4,
        DownloadCompleted = 5,
        BadRequest = 6
    }
}
