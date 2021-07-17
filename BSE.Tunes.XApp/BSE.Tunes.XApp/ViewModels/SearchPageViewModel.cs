using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Extensions;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class SearchPageViewModel : ViewModelBase
    {
        private DelegateCommand<string> _textChangedCommand;
        private DelegateCommand<GridPanel> _selectItemCommand;
        private ICommand _showAllAlbumSearchResultsCommand;
        private ICommand _showAllTrackSearchResultsCommand;
        private DelegateCommand<GridPanel> _playCommand;
        private string _textValue;
        private readonly IDataService _dataService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IPlayerManager _playerManager;
        private ObservableCollection<GridPanel> _albums;
        private ObservableCollection<GridPanel> _tracks;
        private bool _hasAlbums;
        private bool _hasMoreAlbums;
        private bool _hasTracks;
        private bool _hasMoreTracks;

        public DelegateCommand<string> TextChangedCommand => _textChangedCommand
            ?? (_textChangedCommand = new DelegateCommand<string>(TextChanged));

        public DelegateCommand<GridPanel> SelectItemCommand => _selectItemCommand
            ?? (_selectItemCommand = new DelegateCommand<GridPanel>(SelectItem));

        public ICommand ShowAllAlbumSearchResultsCommand => _showAllAlbumSearchResultsCommand
           ?? (_showAllAlbumSearchResultsCommand = new DelegateCommand(ShowAllAlbumSearchResults));

        public DelegateCommand<GridPanel> PlayCommand => _playCommand
            ?? (_playCommand = new DelegateCommand<GridPanel>(PlayTrack));


        public ICommand ShowAllTrackSearchResultsCommand => _showAllTrackSearchResultsCommand
           ?? (_showAllTrackSearchResultsCommand = new DelegateCommand(ShowAllTrackSearchResults));

        public string TextValue
        {
            get
            {
                return _textValue;
            }
            set
            {
                SetProperty<string>(ref _textValue, value);
            }
        }

        public ObservableCollection<GridPanel> Albums => _albums ?? (_albums = new ObservableCollection<GridPanel>());

        public ObservableCollection<GridPanel> Tracks => _tracks ?? (_tracks = new ObservableCollection<GridPanel>());

        public bool HasAlbums
        {
            get
            {
                return _hasAlbums;
            }
            set
            {
                SetProperty<bool>(ref _hasAlbums, value);
            }
        }

        public bool HasMoreAlbums
        {
            get
            {
                return _hasMoreAlbums;
            }
            set
            {
                SetProperty<bool>(ref _hasMoreAlbums, value);
            }
        }

        public bool HasTracks
        {
            get
            {
                return _hasTracks;
            }
            set
            {
                SetProperty<bool>(ref _hasTracks, value);
            }
        }

        public bool HasMoreTracks
        {
            get
            {
                return _hasMoreTracks;
            }
            set
            {
                SetProperty<bool>(ref _hasMoreTracks, value);
            }
        }

        public SearchPageViewModel(INavigationService navigationService,
            IResourceService resourceService,
            IDataService dataService,
            IEventAggregator eventAggregator,
            IPlayerManager playerManager) : base(navigationService, resourceService)
        {
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _playerManager = playerManager;
            IsBusy = false;
            HasAlbums = HasTracks = false;

            _eventAggregator.GetEvent<AlbumInfoSelectionEvent>().ShowAlbum(async (track) =>
            {
                if (PageUtilities.IsCurrentPageTypeOf(typeof(SearchPage)))
                {
                    var navigationParams = new NavigationParameters
                    {
                        { "album", track.Album }
                    };

                    await NavigationService.NavigateAsync(nameof(AlbumDetailPage), navigationParams);
                }
            });
        }

        private async void TextChanged(string searchPhrase)
        {
            IsBusy = true;
            if (string.IsNullOrEmpty(searchPhrase) || searchPhrase.Length < 3)
            {
                HasAlbums = HasTracks = false;
            }
            else
            {
                await GetAlbumResults(searchPhrase);
                await GetTrackResults(searchPhrase);
            }
            IsBusy = false;
        }

        private async Task GetTrackResults(string searchPhrase)
        {
            var tracks = await _dataService.GetTrackSearchResults(searchPhrase, 0, 4);
            if (tracks.Length == 0)
            {
                HasTracks = false;
            }
            else
            {
                HasTracks = true;
                HasMoreTracks = tracks.Length > 3;
                var index = 0;
                var newResults = tracks.Take(3).Reverse();

                foreach (var item in newResults)
                {
                    Tracks.Insert(index, new GridPanel
                    {
                        Title = item.Name,
                        SubTitle = item.Album.Artist.Name,
                        ImageSource = _dataService.GetImage(item.Album.AlbumId, true)?.AbsoluteUri,
                        Data = item
                    });
                    index++;
                }
                if (Tracks.Count > newResults.Count())
                {
                    var c = Tracks.Count;
                    for (int i = c - 1; i >= newResults.Count(); i--)
                    {
                        Tracks.RemoveAt(i);
                    }
                }
            }
        }

        private async Task GetAlbumResults(string searchPhrase)
        {
            var albums = await _dataService.GetAlbumSearchResults(searchPhrase, 0, 4);
            if (albums.Length == 0)
            {
                HasAlbums = false;
            }
            else
            {
                HasAlbums = true;
                HasMoreAlbums = albums.Length > 3;
                var index = 0;
                var newResults = albums.Take(3).Reverse();

                foreach (var item in newResults)
                {
                    Albums.Insert(index, new GridPanel
                    {
                        Title = item.Title,
                        SubTitle = item.Artist.Name,
                        ImageSource = _dataService.GetImage(item.AlbumId, true)?.AbsoluteUri,
                        Data = item
                    });
                    index++;
                }
                if (Albums.Count > newResults.Count())
                {
                    var c = Albums.Count;
                    for (int i = c - 1; i >= newResults.Count(); i--)
                    {
                        Albums.RemoveAt(i);
                    }
                }
            }
        }
        
        private async void SelectItem(GridPanel obj)
        {
            if (obj?.Data is Album album)
            {
                var navigationParams = new NavigationParameters
                    {
                        { "album", album }
                    };
                await NavigationService.NavigateAsync($"{nameof(AlbumDetailPage)}", navigationParams);
            }
        }
        
        private void PlayTrack(GridPanel obj)
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
        
        private async void ShowAllAlbumSearchResults()
        {
            var navigationParams = new NavigationParameters
                    {
                        { "query",  TextValue}
                    };
            await NavigationService.NavigateAsync($"{nameof(AlbumSearchResultsPage)}", navigationParams);
        }
        
        private async void ShowAllTrackSearchResults()
        {
            var navigationParams = new NavigationParameters
                    {
                        { "query",  TextValue}
                    };
            await NavigationService.NavigateAsync($"{nameof(TrackSearchResultsPage)}", navigationParams);
        }
    }
}
