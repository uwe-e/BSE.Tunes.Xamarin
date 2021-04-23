using BSE.Tunes.XApp.Controls;
using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System;

namespace BSE.Tunes.XApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IPlayerManager _playerManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IPageDialogService _pageDialogService;
        private readonly IDataService _dataService;
        private Track _currentTrack;
        private Uri _coverSource;
        private Timer _progressTimer ;
        private DelegateCommand _playCommand;
        private DelegateCommand _pauseCommand;
        private DelegateCommand _playPreviousCommand;
        private DelegateCommand _playNextCommand;
        private AudioPlayerState _audioPlayerState = AudioPlayerState.Closed;
        private AudioPlayerMode _audioPlayerMode = AudioPlayerMode.None;
        private float _progress;

        public DelegateCommand PlayCommand => _playCommand
            ?? (_playCommand = new DelegateCommand(Play, CanPlay));
        public DelegateCommand PauseCommand => _pauseCommand
            ?? (_pauseCommand = new DelegateCommand(Pause, CanPause));
        public DelegateCommand PlayPreviousCommand => _playPreviousCommand
            ?? (_playPreviousCommand = new DelegateCommand(PlayPrevious, CanPlayPrevious));
        public DelegateCommand PlayNextCommand => _playNextCommand
            ?? (_playNextCommand = new DelegateCommand(PlayNext, CanPlayNext));

        public AudioPlayerState AudioPlayerState
        {
            get
            {
                return _audioPlayerState;
            }
            set
            {
                SetProperty<AudioPlayerState>(ref _audioPlayerState, value);
            }
        }

        public AudioPlayerMode AudioPlayerMode
        {
            get
            {
                return _audioPlayerMode;
            }
            set
            {
                SetProperty<AudioPlayerMode>(ref _audioPlayerMode, value);
            }
        }

        public float Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                SetProperty<float>(ref _progress, value);
            }
        }

        public Track CurrentTrack
        {
            get
            {
                return _currentTrack;
            }
            set
            {
                SetProperty<Track>(ref _currentTrack, value);
            }
        }

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

        public MainPageViewModel(INavigationService navigationService,
            IResourceService resourceService,
            IPlayerManager playerManager,
            IEventAggregator eventAggregator,
            IPageDialogService pageDialogService,
            IDataService dataService)
            : base(navigationService, resourceService)
        {
            _playerManager = playerManager;
            _pageDialogService = pageDialogService;
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AudioPlayerStateChangedEvent>().Subscribe((args) => {
                if (args is AudioPlayerState audioPlayerState)
                {
                    if (AudioPlayerState != audioPlayerState)
                    {
                        AudioPlayerState = audioPlayerState;

                        switch (audioPlayerState)
                        {
                            case AudioPlayerState.Stopped:
                            case AudioPlayerState.Paused:
                                _progressTimer?.Stop();
                                break;
                            case AudioPlayerState.Playing:
                                _progressTimer?.Start();
                                break;
                        }
                    }
                };
            }, ThreadOption.UIThread);
            _eventAggregator.GetEvent<MediaStateChangedEvent>().Subscribe((args) => {
                if (args is MediaState mediaState)
                {
                    switch (mediaState)
                    {
                        case MediaState.Opened:
                            OnMediaOpenend();
                            break;
                        case MediaState.Ended:
                            OnMediaEnded();
                            break;
                    }
                };
            }, ThreadOption.UIThread);
            _eventAggregator.GetEvent<TrackChangedEvent>().Subscribe((args) => {
                if (args is Track track)
                {
                    CurrentTrack = track;
                    LoadCoverSource(CurrentTrack);
                }
            }, ThreadOption.UIThread);

            _progressTimer = new Timer(TimeSpan.FromSeconds(1), ProgressTimerAction);
        }

        private void ProgressTimerAction()
        {
            Progress = _playerManager?.Progress ?? default;
        }

        private bool CanPlay()
        {
            return false;
        }

        private async void Play()
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
        
        private bool CanPause()
        {
            return AudioPlayerState == AudioPlayerState.Playing;
        }

        private void Pause()
        {
            _playerManager.Pause();
        }
        
        private bool CanPlayPrevious()
        {
            return _playerManager.CanPlayPreviosTrack();
        }

        private void PlayPrevious()
        {
            _playerManager.PlayPreviousTrack();
        }
        
        private bool CanPlayNext()
        {
            return _playerManager.CanPlayNextTrack();
        }

        private void PlayNext()
        {
            _playerManager.PlayNextTrack();
        }
        
        private void OnMediaEnded()
        {
            _progressTimer?.Stop();
        }

        private void OnMediaOpenend()
        {
            _progressTimer?.Start();
            
            CurrentTrack = _playerManager.CurrentTrack;
            PlayNextCommand.RaiseCanExecuteChanged();
            PauseCommand.RaiseCanExecuteChanged();
            LoadCoverSource(CurrentTrack);
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
