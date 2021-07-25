using BSE.Tunes.XApp.Collections;
using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Extensions;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism.Events;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.ViewModels
{
    public class PlaylistDetailPageViewModel : TracklistBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private readonly IImageService _imageService;
        private Playlist _playlist;

        public Playlist Playlist
        {
            get => _playlist;
            set => SetProperty<Playlist>(ref _playlist, value);
        }

        public PlaylistDetailPageViewModel(
            INavigationService navigationService,
            IResourceService resourceService,
            IFlyoutNavigationService flyoutNavigationService,
            IEventAggregator eventAggregator,
            IDataService dataService,
            ISettingsService settingsService,
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
            _settingsService = settingsService;
            _imageService = imageService;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<PlaylistActionContextChanged>().Subscribe(async args =>
            {
                if (args is PlaylistActionContext managePlaylistContext)
                {
                    if (managePlaylistContext.ActionMode == PlaylistActionMode.PlaylistUpdated)
                    {
                        // if there's a playlistentry that has changed..
                        // and there's no playlistTo object, then it's probably an entry within this current playlist detail that has been removed. 
                        if (managePlaylistContext.PlaylistTo == null && managePlaylistContext.Data is PlaylistEntry playlistEntry)
                        {
                            //if so, then we need a new image
                            Image = null;
                            Image = await _imageService.GetStitchedBitmapSource(Playlist.Id);
                        }

                        if (managePlaylistContext.PlaylistTo?.Id == Playlist.Id)
                        {
                            await LoadPlaylistDetails(managePlaylistContext.PlaylistTo);
                        }
                    }
                    if (managePlaylistContext.ActionMode == PlaylistActionMode.ShowAlbum)
                    {
                        await ShowAlbum(managePlaylistContext);
                    }
                }
            });

            _eventAggregator.GetEvent<AlbumInfoSelectionEvent>().ShowAlbum(async (uniqueTrack) =>
            {
                if (PageUtilities.IsCurrentPageTypeOf(typeof(PlaylistDetailPage)))
                {
                    var navigationParams = new NavigationParameters
                    {
                        { "album", uniqueTrack.Album }
                    };

                    await NavigationService.NavigateAsync(nameof(AlbumDetailPage), navigationParams);
                }
            });
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            Playlist playlist = parameters.GetValue<Playlist>("playlist");
            if (playlist != null)
            {
                await LoadPlaylistDetails(playlist);
            }
            IsBusy = false;
        }

        protected override void PlayTrack(GridPanel obj)
        {
            if (obj?.Data is PlaylistEntry entry)
            {
                PlayTracks(new List<int>
                {
                    entry.TrackId
                }, AudioPlayerMode.Song);
            }
        }

        protected override void PlayAll()
        {
            PlayTracks(GetTrackIds(), AudioPlayerMode.Playlist);
        }

        protected override void PlayAllRandomized()
        {
            PlayTracks(GetTrackIds().ToRandomCollection(), AudioPlayerMode.Playlist);
        }

        protected override ObservableCollection<int> GetTrackIds()
        {
            return new ObservableCollection<int>(Items.Select(track => ((PlaylistEntry)track.Data).TrackId));
        }

        protected override async Task RemoveFromPlaylist(PlaylistActionContext managePlaylistContext)
        {
            await base.RemoveFromPlaylist(managePlaylistContext);
            await UpdateCurrentPlaylist(managePlaylistContext);
        }
        
        private async Task UpdateCurrentPlaylist(PlaylistActionContext managePlaylistContext)
        {
            IsBusy = true;
            if (managePlaylistContext.Data is PlaylistEntry playlistEntry)
            {
                Playlist.Entries.Remove(playlistEntry);

                var playlist = await _dataService.UpdatePlaylist(Playlist);

                GridPanel panel = Items.Where(p => p.Id == playlistEntry.Id).FirstOrDefault<GridPanel>();
                Items.Remove(panel);

                await _imageService.RemoveStitchedBitmaps(playlist.Id);

                managePlaylistContext.ActionMode = PlaylistActionMode.PlaylistUpdated;
                _eventAggregator.GetEvent<PlaylistActionContextChanged>().Publish(managePlaylistContext);

            }
            IsBusy = false;
        }

        private async Task LoadPlaylistDetails(Playlist playlist)
        {
            Items.Clear();
            Image = null;
            Playlist = await _dataService.GetPlaylistById(playlist.Id, _settingsService.User.UserName);
            if (Playlist != null)
            {
                foreach (var entry in Playlist.Entries?.OrderBy(pe => pe.SortOrder))
                {
                    if (entry != null)
                    {
                        Items.Add(new GridPanel
                        {
                            Id = entry.Id,
                            Title = entry.Name,
                            SubTitle = entry.Artist,
                            ImageSource = _imageService.GetBitmapSource(entry.AlbumId, true),
                            Data = entry
                        });
                    }
                }

                Image = await _imageService.GetStitchedBitmapSource(Playlist.Id);

                PlayAllCommand.RaiseCanExecuteChanged();
                PlayAllRandomizedCommand.RaiseCanExecuteChanged();
            }
        }
        
        private async Task ShowAlbum(PlaylistActionContext playlistActionContext)
        {
            if (playlistActionContext?.Data is PlaylistEntry playlistEntry)
            {
                var album = playlistEntry.Track?.Album;
                if (album != null)
                {
                    var navigationParams = new NavigationParameters
                    {
                        {"album", album }
                    };

                    await NavigationService.NavigateAsync($"{nameof(AlbumDetailPage)}", navigationParams);
                }
            }
        }
    }
}
