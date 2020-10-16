using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class PlaylistSelectorDialogPageViewModel : ViewModelBase
    {
        private ICommand _cancelCommand;
        private Album _album;
        private Track _track;
        private ObservableCollection<FlyoutItemViewModel> _playlistFlyoutItems;
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private readonly IImageService _imageService;
        private readonly IEventAggregator _eventAggregator;

        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new DelegateCommand(CloseDialog));
        
        public virtual ObservableCollection<FlyoutItemViewModel> PlaylistFlyoutItems => 
            _playlistFlyoutItems ?? (_playlistFlyoutItems = new ObservableCollection<FlyoutItemViewModel>());

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

        public PlaylistSelectorDialogPageViewModel(
            INavigationService navigationService,
            IResourceService resourceService,
            IDataService dataService,
            ISettingsService settingsService,
            IImageService imageService,
            IEventAggregator eventAggregator) : base(navigationService, resourceService)
        {
            _dataService = dataService;
            _settingsService = settingsService;
            _imageService = imageService;
            _eventAggregator = eventAggregator;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.GetValue<object>("source") is Track track)
            {
                Track = track;
            }
            if (parameters.GetValue<object>("source") is Album album)
            {
                Album = album;
            }
            await CreatePlaylistFlyoutItems();
            IsBusy = false;
            
            base.OnNavigatedTo(parameters);
        }

        protected virtual void AddTracksToPlaylist(Playlist playlist, Track[] tracks)
        {
            if (playlist != null && tracks != null)
            {
                foreach (var track in tracks)
                {
                    if (track != null)
                    {
                        playlist.Entries.Add(new PlaylistEntry
                        {
                            PlaylistId = playlist.Id,
                            TrackId = track.Id,
                            Guid = Guid.NewGuid()
                        });
                    }
                }
                AppendToPlaylist(playlist);
            }
        }

        protected virtual async void AppendToPlaylist(Playlist playlist)
        {
            var changedPlaylist = await _dataService.AppendToPlaylist(playlist);


            CloseDialog();
        }

        private async Task CreatePlaylistFlyoutItems()
        {
            var playlists = await _dataService.GetPlaylistsByUserName(_settingsService.User.UserName, 0, 50);
            if (playlists != null)
            {
                foreach (var playlist in playlists)
                {
                    if (playlist != null)
                    {
                        var flyoutItem = new FlyoutItemViewModel
                        {
                            Text = playlist.Name,
                            ImageSource = await _imageService.GetStitchedBitmapSource(playlist.Id, 50, true),
                            Data = playlist
                        };
                        flyoutItem.ItemClicked += OnFlyoutItemClicked;
                        PlaylistFlyoutItems.Add(flyoutItem);
                    }
                }
            }
        }

        private void OnFlyoutItemClicked(object sender, EventArgs e)
        {
            if (sender is FlyoutItemViewModel flyoutItem)
            {
                if (Album != null)
                {
                    AddTracksToPlaylist(flyoutItem.Data as Playlist, Album.Tracks);
                }
                if (Track != null)
                {
                    AddTracksToPlaylist(flyoutItem.Data as Playlist, new Track[] { Track });
                }
            }
        }

        private async void CloseDialog()
        {
            await NavigationService.GoBackAsync(useModalNavigation: true);
        }
    }
}
