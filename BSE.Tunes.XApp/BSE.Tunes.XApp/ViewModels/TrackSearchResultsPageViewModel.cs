using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Extensions;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism.Events;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.ViewModels
{
    public class TrackSearchResultsPageViewModel : BaseSearchResultsPageViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IPlayerManager _playerManager;

        public TrackSearchResultsPageViewModel(
            INavigationService navigationService,
            IResourceService resourceService,
            IEventAggregator eventAggregator,
            IDataService dataService,
            IPlayerManager playerManager) : base(navigationService, resourceService, dataService)
        {
            _eventAggregator = eventAggregator;
            _playerManager = playerManager;

            _eventAggregator.GetEvent<AlbumInfoSelectionEvent>().ShowAlbum(async (track) =>
            {
                if (PageUtilities.IsCurrentPageTypeOf(typeof(TrackSearchResultsPage)))
                {
                    var navigationParams = new NavigationParameters
                    {
                        { "album", track.Album }
                    };

                    await NavigationService.NavigateAsync(nameof(AlbumDetailPage), navigationParams);
                }
            });
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
                        ImageSource = DataService.GetImage(track.Album.AlbumId, true)?.AbsoluteUri,
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
