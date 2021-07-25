using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Extensions;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class NowPlayingPageViewModel : PlayerBaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly IImageService _imageService;
        private readonly IFlyoutNavigationService _flyoutNavigationService;
        private readonly IEventAggregator _eventAggregator;
        private ICommand _closeDialogCommand;
        private DelegateCommand<object> _openFlyoutCommand;
        private string _coverImage;

        public ICommand CloseDialogCommand => _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand(async () =>
        {
            await CloseDialog();
        }));

        public DelegateCommand<object> OpenFlyoutCommand => _openFlyoutCommand
            ?? (_openFlyoutCommand = new DelegateCommand<object>(OpenFlyout));
        
        public string CoverImage
        {
            get
            {
                return _coverImage;
            }
            set
            {
                SetProperty<string>(ref _coverImage, value);
            }
        }

        public NowPlayingPageViewModel(
            INavigationService navigationService,
            IResourceService resourceService,
            IDataService dataService,
            IPageDialogService pageDialogService,
            IImageService imageService,
            IFlyoutNavigationService flyoutNavigationService,
            IEventAggregator eventAggregator,
            IPlayerManager playerManager) : base(navigationService, resourceService, pageDialogService, playerManager, eventAggregator)
        {
            _dataService = dataService;
            _imageService = imageService;
            _flyoutNavigationService = flyoutNavigationService;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<PlaylistActionContextChanged>().Subscribe(async args =>
            {
                if (args is PlaylistActionContext managePlaylistContext)
                {
                    switch (managePlaylistContext.ActionMode)
                    {
                        case PlaylistActionMode.AddToPlaylist:
                            managePlaylistContext.ActionMode = PlaylistActionMode.None;
                            await AddToPlaylist(managePlaylistContext);
                            break;
                        case PlaylistActionMode.SelectPlaylist:
                            managePlaylistContext.ActionMode = PlaylistActionMode.None;
                            await SelectPlaylist(managePlaylistContext);
                            break;
                        case PlaylistActionMode.CreatePlaylist:
                            managePlaylistContext.ActionMode = PlaylistActionMode.None;
                            await CreateNewPlaylist(managePlaylistContext);
                            break;
                    }
                }
            }, ThreadOption.UIThread);

            _eventAggregator.GetEvent<AlbumInfoSelectionEvent>().ShowAlbum(async (uniqueTrack) =>
            {
                await CloseDialog();
            });
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            var currentTrack = parameters.GetValue<Track>("source");
            if (currentTrack != null)
            {
                CurrentTrack = currentTrack;
                CoverImage = _imageService.GetBitmapSource(CurrentTrack.Album.AlbumId);
            }
            Progress = PlayerManager.Progress;
            AudioPlayerState = PlayerManager.AudioPlayerState;

            base.OnNavigatedTo(parameters);
        }

        protected override void OnTrackChanged(Track currentTrack)
        {
            if (currentTrack != null)
            {
                CoverImage = _imageService.GetBitmapSource(currentTrack.Album.AlbumId);
            }
        }
        
        private async void OpenFlyout(object obj)
        {
            var source = new PlaylistActionContext
            {
                DisplayAlbumInfo = true,
                Data = obj,
            };

            var navigationParams = new NavigationParameters{
                { "source", source }
            };

            await _flyoutNavigationService.ShowFlyoutAsync(nameof(PlaylistActionToolbarPage), navigationParams);
        }
        
        private async Task SelectPlaylist(PlaylistActionContext context)
        {
            await _flyoutNavigationService.CloseFlyoutAsync();
            var navigationParams = new NavigationParameters
            {
                { "source", context }
            };
            await NavigationService.NavigateAsync(nameof(PlaylistSelectorDialogPage), navigationParams, useModalNavigation: true);
        }
        
        private async Task CreateNewPlaylist(PlaylistActionContext context)
        {
            await NavigationService.GoBackAsync(useModalNavigation: true);

            var navigationParams = new NavigationParameters
            {
                { "source", context }
            };
            await NavigationService.NavigateAsync(nameof(NewPlaylistDialogPage), navigationParams, useModalNavigation: true);
        }

        private async Task AddToPlaylist(PlaylistActionContext context)
        {
            await NavigationService.GoBackAsync(useModalNavigation: true);

            if (context.Data is Track track)
            {
                var playlistTo = context.PlaylistTo;
                if (playlistTo != null && track != null)
                {
                    playlistTo.Entries.Add(new PlaylistEntry
                    {
                        PlaylistId = playlistTo.Id,
                        TrackId = track.Id,
                        Guid = Guid.NewGuid()
                    });

                    await _dataService.AppendToPlaylist(playlistTo);
                    await _imageService.RemoveStitchedBitmaps(playlistTo.Id);

                    context.ActionMode = PlaylistActionMode.PlaylistUpdated;
                    _eventAggregator.GetEvent<PlaylistActionContextChanged>().Publish(context);
                }
            }
        }
        
        private async Task CloseDialog()
        {
            await NavigationService.GoBackAsync(useModalNavigation: true, animated: true);
        }
    }
}
