using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class ManageAlbumsPageViewModel : ViewModelBase
    {
        private readonly IFlyoutNavigationService _flyoutNavigationService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDataService _dataService;
        private readonly IStichedBitmapService _stichedBitmapService;
        private ICommand _closeFlyoutCommand;
        private ICommand _addToPlaylistCommand;
        private Track _track;
        private Album _album;
        private string _imageSource;
        private string _subTitle;

        public ICommand CloseFlyoutCommand => _closeFlyoutCommand ?? (_closeFlyoutCommand = new DelegateCommand(CloseFlyout));

        public ICommand AddToPlaylistCommand => _addToPlaylistCommand ?? (_addToPlaylistCommand = new DelegateCommand(AddToPlaylist));

        public Track Track
        {
            get
            {
                return _track;
            }
            set
            {
                SetProperty<Track>(ref _track, value);
            }
        }

        public Album Album
        {
            get
            {
                return _album;
            }
            set
            {
                SetProperty<Album>(ref _album, value);
            }
        }

        public string ImageSource
        {
            get
            {
                return _imageSource;
            }
            set
            {
                SetProperty<string>(ref _imageSource, value);
            }
        }

        public string SubTitle
        {
            get
            {
                return _subTitle;
            }
            set
            {
                SetProperty<string>(ref _subTitle, value);
            }
        }

        public ManageAlbumsPageViewModel(
            INavigationService navigationService,
            IFlyoutNavigationService flyoutNavigationService,
            IEventAggregator eventAggregator,
            IDataService dataService,
            IStichedBitmapService stichedBitmapService,
            IResourceService resourceService) : base(navigationService, resourceService)
        {
            _flyoutNavigationService = flyoutNavigationService;
            _eventAggregator = eventAggregator;
            _dataService = dataService;
            _stichedBitmapService = stichedBitmapService;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.GetValue<object>("source") is Track track)
            {
                Track = track;
                Album = track.Album;
                Title = track.Name;
            }
            if (parameters.GetValue<object>("source") is Album album)
            {
                Album = album;
                Title = album.Title;
            }
            if (parameters.GetValue<object>("source") is PlaylistEntry playlistEntry)
            {
                Title = playlistEntry.Track?.Name;
                SubTitle = playlistEntry.Artist;
                ImageSource = await _stichedBitmapService.GetBitmapSource(playlistEntry.PlaylistId, 50, true);
            }
            if (Album != null)
            {
                SubTitle = Album.Artist.Name;
                ImageSource = _dataService.GetImage(Album.AlbumId, true)?.AbsoluteUri;
            }

            base.OnNavigatedTo(parameters);
        }

        private async void CloseFlyout()
        {
            await _flyoutNavigationService.CloseFlyoutAsync();
        }
        
        private void AddToPlaylist()
        {
            if (Track != null)
            {
                _eventAggregator.GetEvent<AddTrackToPlaylistEvent>().Publish(Track);
            }
            else
            {
                _eventAggregator.GetEvent<AddAlbumToPlaylistEvent>().Publish(Album);
            }
        }
    }
}
