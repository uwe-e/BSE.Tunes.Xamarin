using BSE.Tunes.XApp.Collections;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BSE.Tunes.XApp.ViewModels
{
    public class PlaylistDetailPageViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private readonly ICacheableBitmapService _cacheableBitmapService;
        private readonly IPlayerManager _playerManager;
        private Playlist _playlist;
        private ObservableCollection<GridPanel> _items;
        private string _coverSource;
        private DelegateCommand _playAllCommand;
        private DelegateCommand _playAllRandomizedCommand;
        private DelegateCommand<object> _playCommand;

        public DelegateCommand<object> PlayCommand => _playCommand
            ?? (_playCommand = new DelegateCommand<object>(PlayTrack));

        public DelegateCommand PlayAllCommand => _playAllCommand
            ?? (_playAllCommand = new DelegateCommand(PlayAll, CanPlayAll));

        public DelegateCommand PlayAllRandomizedCommand => _playAllRandomizedCommand
            ?? (_playAllRandomizedCommand = new DelegateCommand(PlayAllRandomized, CanPlayAllRandomized));

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

        public string CoverSource
        {
            get
            {
                return _coverSource;
            }
            set
            {
                SetProperty<string>(ref _coverSource, value);
            }
        }

        public ObservableCollection<GridPanel> Items => _items ?? (_items = new ObservableCollection<GridPanel>());

        public PlaylistDetailPageViewModel(
            INavigationService navigationService,
            IResourceService resourceService,
            IDataService dataService,
            ISettingsService settingsService,
            ICacheableBitmapService cacheableBitmapService,
            IPlayerManager playerManager) : base(navigationService, resourceService)
        {
            _dataService = dataService;
            _settingsService = settingsService;
            _cacheableBitmapService = cacheableBitmapService;
            _playerManager = playerManager;
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

                    CoverSource = await _cacheableBitmapService.GetBitmapSource(
                                        albumIds.Take(4),
                                        Playlist.Guid.ToString(),
                                        500, false);

                    PlayAllCommand.RaiseCanExecuteChanged();
                    PlayAllRandomizedCommand.RaiseCanExecuteChanged();
                }
            }
            IsBusy = false;
        }
        private void PlayTrack(object obj)
        {
            if (obj is PlaylistEntry track)
            {
                PlayTracks(new List<int>
                {
                    track.Id
                }, AudioPlayerMode.Song);
            }
        }
        private bool CanPlayAll()
        {
            return Items.Count > 0;
        }

        private void PlayAll()
        {
            PlayTracks(GetTrackIds(), AudioPlayerMode.Playlist);
        }

        private bool CanPlayAllRandomized()
        {
            return CanPlayAll();
        }

        private void PlayAllRandomized()
        {
            PlayTracks(GetTrackIds().ToRandomCollection(), AudioPlayerMode.Playlist);
        }

        private void PlayTracks(IEnumerable<int> trackIds, AudioPlayerMode audioPlayerMode)
        {
            _playerManager.PlayTracks(
                            new System.Collections.ObjectModel.ObservableCollection<int>(trackIds),
                            audioPlayerMode);
        }

        private ObservableCollection<int> GetTrackIds()
        {
            return new ObservableCollection<int>(Items.Select(track => ((PlaylistEntry)track.Data).TrackId));
        }
    }
}
