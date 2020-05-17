using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Collections;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class AlbumDetailPageViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly IPlayerManager _playerManager;
        private Album _album;
        private ObservableCollection<Track> _tracks;
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

        public ObservableCollection<Track> Tracks => _tracks ?? (_tracks = new ObservableCollection<Track>());

        public AlbumDetailPageViewModel(INavigationService navigationService,
            IResourceService resourceService,
            IDataService dataService,
            IPlayerManager playerManager) : base(navigationService, resourceService)
        {
            _dataService = dataService;
            _playerManager = playerManager;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            Album album = parameters.GetValue<Album>("album");
            if (album != null)
            {
                Album = await _dataService.GetAlbumById(album.Id);
                foreach(Track track in Album.Tracks)
                {
                    Tracks.Add(track);
                }
                CoverSource = _dataService.GetImage(Album.AlbumId)?.AbsoluteUri;
                PlayAllCommand.RaiseCanExecuteChanged();
                IsBusy = false;
            }
        }

        private void PlayTrack(object obj)
        {
            if (obj is Track track)
            {
                PlayTracks(new List<int>
                {
                    track.Id
                }, AudioPlayerMode.Song);
            }
        }
        
        private bool CanPlayAll()
        {
            return Tracks.Count > 0;
        }

        private void PlayAll()
        {
            PlayTracks(GetTrackIds(), AudioPlayerMode.CD);
        }

        private bool CanPlayAllRandomized()
        {
            return CanPlayAll();
        }

        private void PlayAllRandomized()
        {
            PlayTracks(GetTrackIds().ToRandomCollection(), AudioPlayerMode.CD);
        }
        
        private void PlayTracks(IEnumerable<int> trackIds, AudioPlayerMode audioPlayerMode)
        {
            _playerManager.PlayTracks(
                            new System.Collections.ObjectModel.ObservableCollection<int>(trackIds),
                            audioPlayerMode);
        }

        private ObservableCollection<int> GetTrackIds()
        {
            return new ObservableCollection<int>(Tracks.Select(track => track.Id));
        }
    }
}
