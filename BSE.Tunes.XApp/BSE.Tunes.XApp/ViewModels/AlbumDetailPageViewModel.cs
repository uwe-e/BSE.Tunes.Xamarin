using BSE.Tunes.XApp.Collections;
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BSE.Tunes.XApp.ViewModels
{
    public class AlbumDetailPageViewModel : TracklistBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDataService _dataService;
        private readonly IImageService _imageService;
        private readonly int _pageSize;
        private int _pageNumber;
        private bool _hasItems;
        private bool _isQueryBusy;
        private Album _album;
        private GridPanel _selectedAlbum;
        private ICommand _selectAlbumCommand;
        private ICommand _loadMoreAlbumssCommand;
        private ObservableCollection<GridPanel> _albums;
        private bool _hasFurtherAlbums;

        public ICommand LoadMoreAlbumsCommand => _loadMoreAlbumssCommand ?? (
            _loadMoreAlbumssCommand = new DelegateCommand(() =>
            {
                Device.BeginInvokeOnMainThread(async () => await LoadMoreAlbums());
            }));

        public ICommand SelectAlbumCommand => _selectAlbumCommand
            ?? (_selectAlbumCommand = new Command<GridPanel>(SelectAlbum));

        public ObservableCollection<GridPanel> Albums => _albums
            ?? (_albums = new ObservableCollection<GridPanel>());

        public GridPanel SelectedAlbum
        {
            get => _selectedAlbum;
            set => SetProperty<GridPanel>(ref _selectedAlbum, value);
        }

        public Album Album
        {
            get => _album;
            set => SetProperty<Album>(ref _album, value);
        }

        public bool IsQueryBusy
        {
            get
            {
                return _isQueryBusy;
            }
            set
            {
                SetProperty<bool>(ref _isQueryBusy, value);
            }
        }

        public bool HasFurtherAlbums
        {
            get
            {
                return _hasFurtherAlbums;
            }
            set
            {
                SetProperty<bool>(ref _hasFurtherAlbums, value);
            }
        }

        public AlbumDetailPageViewModel(INavigationService navigationService,
            IFlyoutNavigationService flyoutNavigationService,
            IEventAggregator eventAggregator,
            IResourceService resourceService,
            IDataService dataService,
            IImageService imageService,
            IPlayerManager playerManager) : base(
                navigationService,
                resourceService,
                flyoutNavigationService,
                dataService,
                playerManager,
                imageService,
                eventAggregator)
        {
            _dataService = dataService;
            _imageService = imageService;
            _eventAggregator = eventAggregator;
            _pageSize = 10;
            _pageNumber = 0;
            _hasItems = true;
            HasFurtherAlbums = false;

            _eventAggregator.GetEvent<AlbumInfoSelectionEvent>().ShowAlbum(async (track) =>
            {
                if (PageUtilities.IsCurrentPageTypeOf(typeof(AlbumDetailPage)))
                {
                    var navigationParams = new NavigationParameters
                    {
                        { "album", track.Album }
                    };

                    await NavigationService.NavigateAsync($"{nameof(AlbumDetailPage)}", navigationParams);
                }
            });
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            Album album = parameters.GetValue<Album>("album");
            await LoadAlbum(album);
            await LoadMoreAlbums();
        }

        private async Task LoadAlbum(Album album)
        {
            if (album != null)
            {
                Album = await _dataService.GetAlbumById(album.Id);
                foreach (Track track in Album.Tracks)
                {
                    track.Album = new Album
                    {
                        AlbumId = Album.AlbumId,
                        Id = Album.Id,
                        Title = Album.Title,
                        Artist = Album.Artist
                    };
                    Items.Add(new GridPanel
                    {
                        Number = track.TrackNumber,
                        Title = track.Name,
                        Data = track

                    });
                }
                Image = _imageService.GetBitmapSource(Album.AlbumId);
                PlayAllCommand.RaiseCanExecuteChanged();
                PlayAllRandomizedCommand.RaiseCanExecuteChanged();
                IsBusy = false;
            }
        }

        private async Task LoadMoreAlbums()
        {
            if (IsQueryBusy)
            {
                return;
            }
            
            if (_hasItems)
            {
                IsQueryBusy = true;
                try
                {
                    var albums = await _dataService.GetAlbumsByArtist(Album.Artist.Id, _pageNumber, _pageSize);
                    if (albums == null || albums.Count == 0)
                    {
                        _hasItems = false;
                        return;
                    }
                    foreach (var album in albums)
                    {
                        if (album != null)
                        {
                            Albums.Add(new GridPanel
                            {
                                Title = album.Title,
                                SubTitle = album.Artist.Name,
                                ImageSource = _imageService.GetBitmapSource(album.AlbumId),
                                Data = album
                            });
                        }
                    }
                    if (Albums.Count > 1)
                    {
                        HasFurtherAlbums = true;
                    }
                    _pageNumber = Albums.Count;
                }
                finally {
                    IsQueryBusy = false;
                }
            }
        }

        private async void SelectAlbum(GridPanel obj)
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

        protected override void PlayTrack(GridPanel obj)
        {
            if (obj?.Data is Track track)
            {
                PlayTracks(new List<int>
                {
                    track.Id
                }, AudioPlayerMode.Song);
            }
        }

        protected override void PlayAll()
        {
            PlayTracks(GetTrackIds(), AudioPlayerMode.CD);
        }

        protected override void PlayAllRandomized()
        {
            PlayTracks(GetTrackIds().ToRandomCollection(), AudioPlayerMode.CD);
        }

        protected override ObservableCollection<int> GetTrackIds()
        {
            return new ObservableCollection<int>(Items.Select(track => ((Track)track.Data).Id));
        }
    }
}
