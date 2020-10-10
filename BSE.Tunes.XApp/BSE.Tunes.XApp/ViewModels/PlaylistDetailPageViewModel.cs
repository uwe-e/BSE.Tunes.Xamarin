using BSE.Tunes.XApp.Collections;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BSE.Tunes.XApp.ViewModels
{
    public class PlaylistDetailPageViewModel : TracklistBaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private readonly IStichedBitmapService _cacheableBitmapService;
        private Playlist _playlist;

        public Playlist Playlist
        {
            get
            {
                return _playlist;
            }
            set
            {
                SetProperty<Playlist>(ref _playlist, value);
            }
        }

        public PlaylistDetailPageViewModel(
            INavigationService navigationService,
            IResourceService resourceService,
            IFlyoutNavigationService flyoutNavigationService,
            IEventAggregator eventAggregator,
            IDataService dataService,
            ISettingsService settingsService,
            IStichedBitmapService cacheableBitmapService,
            IPlayerManager playerManager) : base(navigationService, resourceService, flyoutNavigationService, playerManager, eventAggregator)
        {
            _dataService = dataService;
            _settingsService = settingsService;
            _cacheableBitmapService = cacheableBitmapService;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            Playlist playlist = parameters.GetValue<Playlist>("playlist");
            Collection<Guid> albumIds = new Collection<Guid>(); ;
            if (playlist != null)
            {
                Playlist = await _dataService.GetPlaylistById(playlist.Id, _settingsService.User.UserName);
                if (Playlist != null)
                {
                    foreach (var entry in Playlist.Entries?.OrderBy(pe => pe.SortOrder))
                    {
                        if (entry != null)
                        {
                            Items.Add(new GridPanel
                            {
                                Title = entry.Name,
                                SubTitle = entry.Artist,
                                ImageSource = _dataService.GetImage(entry.AlbumId, true)?.AbsoluteUri,
                                Data = entry
                            });
                            albumIds.Add(entry.AlbumId);
                        }
                    }

                    Image = await _cacheableBitmapService.GetBitmapSource(
                                        Playlist.Id);

                    //Image = await _cacheableBitmapService.GetBitmapSource(
                    //                    albumIds.Take(4),
                    //                    Playlist.Guid.ToString(),
                    //                    500, false);
                    PlayAllCommand.RaiseCanExecuteChanged();
                    PlayAllRandomizedCommand.RaiseCanExecuteChanged();
                }
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
    }
}
