using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.ViewModels
{
    public class TrackSearchResultsPageViewModel : BaseSearchResultsPageViewModel
    {
        private readonly IPlayerManager _playerManager;

        public TrackSearchResultsPageViewModel(
            INavigationService navigationService,
            IResourceService resourceService,
            IDataService dataService,
            IPlayerManager playerManager) : base(navigationService, resourceService, dataService)
        {
            _playerManager = playerManager;
        }

        protected override async Task GetSearchResults()
        {
            var tracks = await DataService.GetTrackSearchResults(Query, PageNumber, PageSize);
            if (tracks.Length == 0)
            {
                HasItems = false;
            }

            foreach (var track in tracks)
            {
                if (track != null)
                {
                    Items.Add(new GridPanel
                    {
                        Title = track.Name,
                        SubTitle = track.Album.Artist.Name,
                        ImageSource = DataService.GetImage(track.Album.AlbumId)?.AbsoluteUri,
                        Data = track
                    });
                }
            }
            PageNumber = Items.Count;
        }

        protected override void SelectItem(GridPanel obj)
        {
            if (obj?.Data is Track track)
            {
                _playerManager.PlayTracks(new ObservableCollection<int>()
                {
                    track.Id
                }
                , AudioPlayerMode.Song);
            }
        }
    }
}
