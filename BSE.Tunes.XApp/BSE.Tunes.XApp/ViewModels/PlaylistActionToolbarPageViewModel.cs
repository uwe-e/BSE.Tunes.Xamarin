using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System.Diagnostics;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class PlaylistActionToolbarPageViewModel : ViewModelBase
    {
        private readonly IFlyoutNavigationService _flyoutNavigationService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IImageService _imageService;
        private ICommand _closeFlyoutCommand;
        private ICommand _addToPlaylistCommand;
        private ICommand _removeFromPlaylistCommand;
        private ICommand _removePlaylistCommand;
        private string _imageSource;
        private string _subTitle;
        private PlaylistActionContext _playlistActionContext;
        private bool _canRemovePlaylist;
        private bool _canRemoveFromPlaylist;

        public ICommand CloseFlyoutCommand => _closeFlyoutCommand
            ?? (_closeFlyoutCommand = new DelegateCommand(CloseFlyout));

        public ICommand AddToPlaylistCommand => _addToPlaylistCommand
            ?? (_addToPlaylistCommand = new DelegateCommand(AddToPlaylist));

        public ICommand RemoveFromPlaylistCommand => _removeFromPlaylistCommand
            ?? (_removeFromPlaylistCommand = new DelegateCommand(RemoveFromPlaylist));

        public ICommand RemovePlaylistCommand => _removePlaylistCommand
            ?? (_removePlaylistCommand = new DelegateCommand(RemovePlaylist));

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

        public bool CanRemovePlaylist
        {
            get
            {
                return _canRemovePlaylist;
            }
            set
            {
                SetProperty<bool>(ref _canRemovePlaylist, value);
            }
        }

        public bool CanRemoveFromPlaylist
        {
            get
            {
                return _canRemoveFromPlaylist;
            }
            set
            {
                SetProperty<bool>(ref _canRemoveFromPlaylist, value);
            }
        }

        public PlaylistActionToolbarPageViewModel(
            INavigationService navigationService,
            IFlyoutNavigationService flyoutNavigationService,
            IEventAggregator eventAggregator,
            IImageService imageService,
            IResourceService resourceService) : base(navigationService, resourceService)
        {
            _flyoutNavigationService = flyoutNavigationService;
            _eventAggregator = eventAggregator;
            _imageService = imageService;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            _playlistActionContext = parameters.GetValue<PlaylistActionContext>("source");
            if (_playlistActionContext?.Data is Track track)
            {
                Title = track.Name;
                SubTitle = track.Album.Artist.Name;
                ImageSource = _imageService.GetBitmapSource(track.Album.AlbumId, true);
            }
            if (_playlistActionContext?.Data is Album album)
            {
                Title = album.Title;
                SubTitle = album.Artist.Name;
                ImageSource = _imageService.GetBitmapSource(album.AlbumId, true);
            }
            if (_playlistActionContext?.Data is Playlist playlist)
            {
                CanRemovePlaylist = true;
                Title = playlist.Name;
                ImageSource = await _imageService.GetStitchedBitmapSource(playlist.Id, 50, true);
            }
            if (_playlistActionContext?.Data is PlaylistEntry playlistEntry)
            {
                CanRemoveFromPlaylist = true;
                Title = playlistEntry.Track?.Name;
                SubTitle = playlistEntry.Artist;
                ImageSource = _imageService.GetBitmapSource(playlistEntry.AlbumId, true);
            }

            base.OnNavigatedTo(parameters);
        }

        private async void CloseFlyout()
        {
            await _flyoutNavigationService.CloseFlyoutAsync();
        }
        
        private void AddToPlaylist()
        {
            if (_playlistActionContext != null)
            {
                _playlistActionContext.ActionMode = PlaylistActionMode.SelectPlaylist;
                _eventAggregator.GetEvent<PlaylistActionContextChanged>().Publish(_playlistActionContext);
            }
        }
        
        private void RemovePlaylist()
        {
            if (_playlistActionContext != null)
            {
                _playlistActionContext.ActionMode = PlaylistActionMode.RemovePlaylist;
                _eventAggregator.GetEvent<PlaylistActionContextChanged>().Publish(_playlistActionContext);
            }
        }

        private void RemoveFromPlaylist()
        {
            if (_playlistActionContext != null)
            {
                _playlistActionContext.ActionMode = PlaylistActionMode.RemoveFromPlaylist;
                _eventAggregator.GetEvent<PlaylistActionContextChanged>().Publish(_playlistActionContext);
            }
        }
    }
}
