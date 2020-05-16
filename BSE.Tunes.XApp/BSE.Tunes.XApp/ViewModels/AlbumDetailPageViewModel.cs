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
        private DelegateCommand _playCommand;
        private DelegateCommand _playRandomCommand;

        public DelegateCommand PlayCommand => _playCommand
            ?? (_playCommand = new DelegateCommand(PlayAll, CanPlayAll));

        public DelegateCommand PlayRandomCommand => _playRandomCommand
            ?? (_playRandomCommand = new DelegateCommand(PlayRandom, CanPlayRandom));

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
                PlayCommand.RaiseCanExecuteChanged();
                IsBusy = false;
            }
        }
        
        private bool CanPlayAll()
        {
            return Tracks.Count > 0;
        }

        private void PlayAll()
        {
            PlayTracks(GetTrackIds());
        }

        private bool CanPlayRandom()
        {
            return CanPlayAll();
        }

        private void PlayRandom()
        {
            PlayTracks(GetTrackIds().ToRandomCollection());
        }
        
        private void PlayTracks(IEnumerable<int> trackIds)
        {
            _playerManager.PlayTracks(
                            new System.Collections.ObjectModel.ObservableCollection<int>(trackIds),
                            AudioPlayerMode.CD);
        }

        private ObservableCollection<int> GetTrackIds()
        {
            return new ObservableCollection<int>(Tracks.Select(track => track.Id));
        }
    }
}
