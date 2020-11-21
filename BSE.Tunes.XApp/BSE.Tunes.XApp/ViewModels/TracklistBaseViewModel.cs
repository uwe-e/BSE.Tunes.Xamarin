using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BSE.Tunes.XApp.ViewModels
{
    public class TracklistBaseViewModel : ViewModelBase
    {
        private readonly IFlyoutNavigationService _flyoutNavigationService;
        private readonly IDataService _dataService;
        private readonly IPlayerManager _playerManager;
        private readonly IImageService _imageService;
        private readonly IPlaylistManager _playlistManager;
        private readonly IEventAggregator _eventAggregator;
        private ObservableCollection<GridPanel> _items;
        private string _image;
        private DelegateCommand<object> _openFlyoutCommand;
        private DelegateCommand _playAllCommand;
        private DelegateCommand _playAllRandomizedCommand;
        private DelegateCommand<GridPanel> _playCommand;

        public DelegateCommand<GridPanel> PlayCommand => _playCommand
            ?? (_playCommand = new DelegateCommand<GridPanel>(PlayTrack));

        public DelegateCommand PlayAllCommand => _playAllCommand
            ?? (_playAllCommand = new DelegateCommand(PlayAll, CanPlayAll));

        public DelegateCommand PlayAllRandomizedCommand => _playAllRandomizedCommand
            ?? (_playAllRandomizedCommand = new DelegateCommand(PlayAllRandomized, CanPlayAllRandomized));

        public DelegateCommand<object> OpenFlyoutCommand => _openFlyoutCommand
            ?? (_openFlyoutCommand = new DelegateCommand<object>(OpenFlyout));

        protected IFlyoutNavigationService FlyoutNavigationService => _flyoutNavigationService;

        public ObservableCollection<GridPanel> Items => _items ?? (_items = new ObservableCollection<GridPanel>());

        public string Image
        {
            get
            {
                return _image;
            }
            set
            {
                SetProperty<string>(ref _image, value);
            }
        }

        protected IPlaylistManager PlaylistManager
        {
            get { return _playlistManager; }
        }

        public TracklistBaseViewModel(
            INavigationService navigationService,
            IResourceService resourceService,
            IFlyoutNavigationService flyoutNavigationService,
            IDataService dataService,
            IPlayerManager playerManager,
            IImageService imageService,
            IEventAggregator eventAggregator) : base(navigationService, resourceService)
        {
            _flyoutNavigationService = flyoutNavigationService;
            _dataService = dataService;
            _playerManager = playerManager;
            _imageService = imageService;
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
                        case PlaylistActionMode.RemoveFromPlaylist:
                            managePlaylistContext.ActionMode = PlaylistActionMode.None;
                            await RemoveFromPlaylist(managePlaylistContext);
                            break;
                        case PlaylistActionMode.RemovePlaylist:
                            managePlaylistContext.ActionMode = PlaylistActionMode.None;
                            await RemovePlaylist(managePlaylistContext);
                            break;
                        case PlaylistActionMode.PlaylistDeleted:
                            managePlaylistContext.ActionMode = PlaylistActionMode.None;
                            await NavigationService.NavigateAsync($"/{ nameof(NavigationPage)}/{nameof(PlaylistsPage)}");
                            break;
                        //case PlaylistActionMode.PlaylistUpdated:
                        //    managePlaylistContext.ActionMode = PlaylistActionMode.None;
                        //    break;
                    }
                    //if (managePlaylistContext.ActionMode == PlaylistActionMode.Append)
                    //{
                    //    if (managePlaylistContext.PlaylistTo == null)
                    //    {
                    //        SelectPlaylist(managePlaylistContext);
                    //    }
                    //    else
                    //    {
                    //        AddToPlaylist(managePlaylistContext);
                    //    }

                    //}
                    //if (managePlaylistContext.ActionMode == PlaylistActionMode.Remove)
                    //{
                    //    RemoveFromPlaylist(managePlaylistContext);
                    //}
                    //if (managePlaylistContext.ActionMode == PlaylistActionMode.New)
                    //{
                    //    CreateNewPlaylist(managePlaylistContext);
                    //}
                    //if (managePlaylistContext.ActionMode == PlaylistActionMode.Deleted)
                    //{
                    //    await NavigationService.NavigateAsync($"{ nameof(NavigationPage)}/{nameof(PlaylistsPage)}");
                    //}
                    //if (managePlaylistContext.ActionMode == PlaylistActionMode.Updated)
                    //{
                    //    //var playlist = managePlaylistContext.PlaylistTo;
                    //    //if (playlist != null)
                    //    //{
                    //    //    await _imageService.RemoveStitchedBitmaps(playlist.Id);
                    //    //}
                    //    //managePlaylistContext.ActionMode = PlaylistActionMode.None;
                    //}

                }

            },ThreadOption.UIThread);
        }

        protected virtual void PlayTrack(GridPanel obj)
        {
        }

        protected virtual void PlayAll()
        {
        }

        protected virtual bool CanPlayAll()
        {
            return Items.Count > 0;
        }

        protected virtual bool CanPlayAllRandomized()
        {
            return CanPlayAll();
        }

        protected virtual void PlayAllRandomized()
        {
        }

        protected virtual void PlayTracks(IEnumerable<int> trackIds, AudioPlayerMode audioPlayerMode)
        {
            _playerManager.PlayTracks(
                            new ObservableCollection<int>(trackIds),
                            audioPlayerMode);
        }

        protected virtual ObservableCollection<int> GetTrackIds()
        {
            throw new System.NotImplementedException();
        }

        protected async virtual void OpenFlyout(object obj)
        {
            var source = new PlaylistActionContext
            {
                Data = obj,
            };

            if (obj is GridPanel item)
            {
                source.Data = item.Data;
            }

            var navigationParams = new NavigationParameters{
                        { "source", source }
            };

            Debug.Print($"{nameof(OpenFlyout)} - new PlaylistActionContext created");

            await FlyoutNavigationService.ShowFlyoutAsync(nameof(PlaylistActionToolbarPage), navigationParams);
        }

        protected async Task SelectPlaylist(PlaylistActionContext managePlaylistContext)
        {
            await _flyoutNavigationService.CloseFlyoutAsync();

            var navigationParams = new NavigationParameters
            {
                { "source", managePlaylistContext }
            };
            await NavigationService.NavigateAsync(nameof(PlaylistSelectorDialogPage), navigationParams, useModalNavigation: true);
        }

        protected async virtual Task AddToPlaylist(PlaylistActionContext managePlaylistContext)
        {
            await NavigationService.GoBackAsync(useModalNavigation: true);

            IEnumerable<Track> tracks = default;

            if (managePlaylistContext.Data is Track track)
            {
                tracks = Enumerable.Repeat(track, 1);
            }
            if (managePlaylistContext.Data is Album album)
            {
                tracks = album.Tracks;
            }
            if (managePlaylistContext.Data is PlaylistEntry playlistEntry)
            {
                tracks = Enumerable.Repeat(playlistEntry.Track, 1);
            }
            if (managePlaylistContext.Data is Playlist playlist)
            {
                tracks = playlist.Entries?.Select(t => t.Track);
            }

            if (tracks != null)
            {
                await AddTracksToPlaylist(managePlaylistContext, tracks);
            }

        }

        private async Task AddTracksToPlaylist(PlaylistActionContext managePlaylistContext, IEnumerable<Track> tracks)
        {
            var playlistTo = managePlaylistContext.PlaylistTo;
            if (playlistTo != null && tracks != null)
            {
                foreach (var track in tracks)
                {
                    if (track != null)
                    {
                        playlistTo.Entries.Add(new PlaylistEntry
                        {
                            PlaylistId = playlistTo.Id,
                            TrackId = track.Id,
                            Guid = Guid.NewGuid()
                        });
                    }
                }
                await _dataService.AppendToPlaylist(playlistTo);

                managePlaylistContext.ActionMode = PlaylistActionMode.PlaylistUpdated;
                _eventAggregator.GetEvent<PlaylistActionContextChanged>().Publish(managePlaylistContext);
            }
        }

        protected virtual Task UpdatePlaylist(PlaylistActionContext managePlaylistContext) {
            return null;
        }
        
        private async Task RemoveFromPlaylist(PlaylistActionContext managePlaylistContext)
        {
            await _flyoutNavigationService.CloseFlyoutAsync();
            await UpdatePlaylist(managePlaylistContext);
        }

        private async Task RemovePlaylist(PlaylistActionContext managePlaylistContext)
        {
            await _flyoutNavigationService.CloseFlyoutAsync();

            if (managePlaylistContext.Data is Playlist playlist)
            {
                await _dataService.DeletePlaylist(playlist.Id);

                managePlaylistContext.ActionMode = PlaylistActionMode.PlaylistDeleted;

                _eventAggregator.GetEvent<PlaylistActionContextChanged>().Publish(managePlaylistContext);
            }
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
    }
}
