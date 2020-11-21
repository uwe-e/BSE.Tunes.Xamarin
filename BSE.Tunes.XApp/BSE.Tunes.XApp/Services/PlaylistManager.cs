namespace BSE.Tunes.XApp.Services
{
    public class PlaylistManager : IPlaylistManager
    {
        private readonly IDataService _dataService;

        public PlaylistManager(IDataService dataService)
        {
            _dataService = dataService;
        }
    }
}
