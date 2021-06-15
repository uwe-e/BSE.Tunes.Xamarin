using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System;

namespace BSE.Tunes.XApp.ViewModels
{
    public class MainPageViewModel : PlayerBaseViewModel
    {
        private readonly IPlayerManager _playerManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IPageDialogService _pageDialogService;
        private readonly IDataService _dataService;
        private Uri _coverSource;
        private DelegateCommand<Track> _selectTrackCommand;

        public DelegateCommand<Track> SelectTrackCommand => _selectTrackCommand
            ?? (_selectTrackCommand = new DelegateCommand<Track>(SelectTrack, CanSelectTrack));

        public Uri CoverSource
        {
            get
            {
                return _coverSource;
            }
            set
            {
                SetProperty<Uri>(ref _coverSource, value);
            }
        }

        public MainPageViewModel(
            INavigationService navigationService,
            IResourceService resourceService,
            IPlayerManager playerManager,
            IEventAggregator eventAggregator,
            IPageDialogService pageDialogService,
            IDataService dataService)
            : base(navigationService, resourceService, pageDialogService, playerManager, eventAggregator)
        {
            _playerManager = playerManager;
            _pageDialogService = pageDialogService;
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<TrackChangedEvent>().Subscribe((args) =>
            {
                if (args is Track track)
                {
                    CurrentTrack = track;
                    LoadCoverSource(CurrentTrack);
                }
            }, ThreadOption.UIThread);
            _eventAggregator.GetEvent<PlaylistActionContextChanged>().Subscribe(async args =>
            {
                if (args is PlaylistActionContext managePlaylistContext)
                {
                    if (managePlaylistContext.ActionMode == PlaylistActionMode.ShowAlbum)
                    {
                        if (managePlaylistContext.Data is Track track)
                        {
                            var navigationParams = new NavigationParameters
                            {
                                { "album", track.Album }
                            };
                            //await NavigationService.GoBackAsync(useModalNavigation: true);
                            //await NavigationService.GoBackAsync(useModalNavigation: true);
                            //await NavigationService.NavigateAsync($"{nameof(MainPage)}?{KnownNavigationParameters.SelectedTab}={nameof(AlbumsPage)}", navigationParams, false);
                            //await NavigationService.NavigateAsync($"{nameof(AlbumsPage)}/{nameof(AlbumDetailPage)}", navigationParams, false);
                        }
                    }
                }
            });
        }

        private bool CanSelectTrack(Track currentTrack)
        {
            return currentTrack != null;
        }

        private async void SelectTrack(Track currentTrack)
        {
            var navigationParams = new NavigationParameters
            {
                { "source", currentTrack }
            };
            await NavigationService.NavigateAsync(nameof(NowPlayingPage), navigationParams, useModalNavigation: true);
        }

        protected override async void Play()
        {
            try
            {
                switch (AudioPlayerState)
                {
                    case AudioPlayerState.Closed:
                    case AudioPlayerState.Stopped:
                        if (_playerManager.CanPlay())
                        {
                            _playerManager.PlayTracks(AudioPlayerMode.Playlist);
                        }
                        break;
                    case AudioPlayerState.Paused:
                        _playerManager.Play();

                        break;
                }
            }
            catch(Exception exception)
            {
                var dialogResult = ResourceService.GetString("Dialog_Result_Ok");
                await _pageDialogService.DisplayAlertAsync("", exception.Message, dialogResult);
            }
        }
        
        protected override void OnTrackChanged(Track currentTrack)
        {
            LoadCoverSource(currentTrack);
        }

        private void LoadCoverSource(Track track)
        {
            if (track != null)
            {
                Uri coverSource = _dataService.GetImage(track.Album.AlbumId, true);
                if (coverSource != null && !coverSource.Equals(CoverSource))
                {
                    CoverSource = coverSource;
                }
            }
        }
    }
}
