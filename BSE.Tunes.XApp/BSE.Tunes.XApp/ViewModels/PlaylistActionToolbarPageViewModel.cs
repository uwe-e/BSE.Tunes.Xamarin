using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System.Threading.Tasks;
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
        private ICommand _displayAlbumInfoCommand;
        private string _imageSource;
        private string _subTitle;
        private PlaylistActionContext _playlistActionContext;
        private bool _canRemovePlaylist;
        private bool _canRemoveFromPlaylist;
        private bool _canDisplayAlbumInfo;

        public ICommand CloseFlyoutCommand => _closeFlyoutCommand
            ?? (_closeFlyoutCommand = new DelegateCommand(async () =>
            {
                await CloseFlyout();
            }));

        public ICommand AddToPlaylistCommand => _addToPlaylistCommand
            ?? (_addToPlaylistCommand = new DelegateCommand(AddToPlaylist));

        public ICommand RemoveFromPlaylistCommand => _removeFromPlaylistCommand
            ?? (_removeFromPlaylistCommand = new DelegateCommand(RemoveFromPlaylist));

        public ICommand RemovePlaylistCommand => _removePlaylistCommand
            ?? (_removePlaylistCommand = new DelegateCommand(RemovePlaylist));

        public ICommand DisplayAlbumInfoCommand => _displayAlbumInfoCommand
            ?? (_displayAlbumInfoCommand = new DelegateCommand(ShowAlbum));

        public string ImageSource
        {
            get => _imageSource;
            set => SetProperty<string>(ref _imageSource, value);
        }

        public string SubTitle
        {
            get => _subTitle;
            set => SetProperty<string>(ref _subTitle, value);
        }

        public bool CanRemovePlaylist
        {
            get => _canRemovePlaylist;
            set => SetProperty<bool>(ref _canRemovePlaylist, value);
        }

        public bool CanRemoveFromPlaylist
        {
            get => _canRemoveFromPlaylist;
            set => SetProperty<bool>(ref _canRemoveFromPlaylist, value);
        }

        public bool CanDisplayAlbumInfo
        {
            get => _canDisplayAlbumInfo;
            set => SetProperty<bool>(ref _canDisplayAlbumInfo, value);
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

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            _playlistActionContext = parameters.GetValue<PlaylistActionContext>("source");
            if (_playlistActionContext?.Data is Track track)
            {
                //comes from the nowplaying page and is used only there
                CanDisplayAlbumInfo = (bool)_playlistActionContext?.DisplayAlbumInfo;
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
                CanDisplayAlbumInfo = true;
                Title = playlistEntry.Track?.Name;
                SubTitle = playlistEntry.Artist;
                ImageSource = _imageService.GetBitmapSource(playlistEntry.AlbumId, true);
            }

            base.OnNavigatedTo(parameters);
        }

        private async Task CloseFlyout()
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
        
        private async void ShowAlbum()
        {
            await CloseFlyout();
            
            if (_playlistActionContext != null)
            {
                if (_playlistActionContext.Data is Track track)
                {
                    _eventAggregator.GetEvent<AlbumInfoSelectionEvent>().Publish(track);
                }
            }
        }
    }
}
