using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class NewPlaylistDialogPageViewModel : ViewModelBase
    {
        private ICommand _cancelCommand;
        private DelegateCommand _saveCommand;
        private string _imageSource;
        private string _playlistName;
        private PlaylistActionContext _playlistActionContext;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private readonly IImageService _imageService;

        public ICommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(CloseDialog));

        public DelegateCommand SaveCommand =>
            _saveCommand ?? (_saveCommand = new DelegateCommand(SavePlaylist, CanSavePlaylist));

        public string ImageSource
        {
            get => _imageSource;
            set => SetProperty<string>(ref _imageSource, value);
        }

        public string PlaylistName
        {
            get => _playlistName;
            set
            {
                SetProperty<string>(ref _playlistName, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public NewPlaylistDialogPageViewModel(
            INavigationService navigationService,
            IResourceService resourceService,
            IDataService dataService,
            ISettingsService settingsService,
            IImageService imageService,
            IEventAggregator eventAggregator) : base(navigationService, resourceService)
        {
            _eventAggregator = eventAggregator;
            _dataService = dataService;
            _settingsService = settingsService;
            _imageService = imageService;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            _playlistActionContext = parameters.GetValue<PlaylistActionContext>("source");

            if (_playlistActionContext?.Data is Track track)
            {
                ImageSource = _imageService.GetBitmapSource(track.Album.AlbumId);
            }
            if (_playlistActionContext?.Data is Album album)
            {
                ImageSource = _imageService.GetBitmapSource(album.AlbumId);
            }
            if (_playlistActionContext?.Data is Playlist playlist)
            {
                ImageSource = await _imageService.GetStitchedBitmapSource(playlist.Id, 50);
            }
            if (_playlistActionContext?.Data is PlaylistEntry playlistEntry)
            {
                ImageSource = _imageService.GetBitmapSource(playlistEntry.AlbumId);
            }

            IsBusy = false;
            
            base.OnNavigatedTo(parameters);
        }

        private async void CloseDialog()
        {
            await NavigationService.GoBackAsync(useModalNavigation: true);
        }
        private bool CanSavePlaylist()
        {
            return !String.IsNullOrEmpty(PlaylistName);
        }

        private async void SavePlaylist()
        {
            try
            {
                var playlist = await _dataService.InsertPlaylist(new Playlist
                {
                    Name = PlaylistName,
                    UserName = _settingsService.User.UserName,
                    Guid = Guid.NewGuid()
                });
                _playlistActionContext.ActionMode = PlaylistActionMode.AddToPlaylist;
                _playlistActionContext.PlaylistTo = playlist as Playlist;
                _eventAggregator.GetEvent<PlaylistActionContextChanged>().Publish(_playlistActionContext);
            }
            catch(Exception ex)
            {

            }
        }
    }
}
