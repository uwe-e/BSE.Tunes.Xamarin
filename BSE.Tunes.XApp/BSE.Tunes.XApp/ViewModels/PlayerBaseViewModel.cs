using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class PlayerBaseViewModel : ViewModelBase
    {
        private readonly IPageDialogService _pageDialogService;
        private readonly IPlayerManager _playerManager;
        private readonly IEventAggregator _eventAggregator;
        private Track _currentTrack;
        private DelegateCommand _playPreviousCommand;
        private DelegateCommand _playCommand;
        private DelegateCommand _pauseCommand;
        private DelegateCommand _playNextCommand;
        private float _progress;
        private AudioPlayerState _audioPlayerState;

        public DelegateCommand PlayPreviousCommand => _playPreviousCommand
            ?? (_playPreviousCommand = new DelegateCommand(PlayPrevious, CanPlayPrevious));

        public DelegateCommand PlayNextCommand => _playNextCommand
            ?? (_playNextCommand = new DelegateCommand(PlayNext, CanPlayNext));

        public DelegateCommand PlayCommand => _playCommand
            ?? (_playCommand = new DelegateCommand(Play));

        public DelegateCommand PauseCommand => _pauseCommand
            ?? (_pauseCommand = new DelegateCommand(Pause, CanPause));

        public IPlayerManager PlayerManager
        {
            get
            {
                return _playerManager;
            }
        }

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

        public Track CurrentTrack
        {
            get { return _currentTrack; }
            set
            {
                SetProperty<Track>(ref _currentTrack, value);
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

        public PlayerBaseViewModel(
            INavigationService navigationService,
            IResourceService resourceService,
            IPageDialogService pageDialogService,
            IPlayerManager playerManager,
            IEventAggregator eventAggregator) : base(navigationService, resourceService)
        {
            _pageDialogService = pageDialogService;
            _playerManager = playerManager;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AudioPlayerStateChangedEvent>().Subscribe((args) =>
            {
                if (args is AudioPlayerState audioPlayerState)
                {
                    if (AudioPlayerState != audioPlayerState)
                    {
                        AudioPlayerState = audioPlayerState;
                    }
                };
            }, ThreadOption.UIThread);
            _eventAggregator.GetEvent<MediaStateChangedEvent>().Subscribe((args) =>
            {
                if (args is MediaState mediaState)
                {
                    switch (mediaState)
                    {
                        case MediaState.Opened:
                            OnMediaOpenend();
                            break;
                        case MediaState.Ended:
                            //OnMediaEnded();
                            break;
                    }
                };
            }, ThreadOption.UIThread);
            _eventAggregator.GetEvent<MediaProgressChangedEvent>().Subscribe((args) =>
            {
                Progress = args.Progress;
            });
        }

        private void OnMediaOpenend()
        {
            CurrentTrack = _playerManager.CurrentTrack;
            PlayPreviousCommand.RaiseCanExecuteChanged();
            PlayNextCommand.RaiseCanExecuteChanged();
            OnTrackChanged(CurrentTrack);
        }

        protected virtual void OnTrackChanged(Track currentTrack)
        {
        }

        private bool CanPlayPrevious()
        {
            return PlayerManager.CanPlayPreviosTrack();
        }
        
        private void PlayPrevious()
        {
            PlayerManager.PlayPreviousTrack();
        }

        protected virtual async void Play()
        {
            try
            {
                switch (AudioPlayerState)
                {
                    case AudioPlayerState.Closed:
                    case AudioPlayerState.Stopped:
                        if (_playerManager.CanPlay())
                        {
                            PlayerManager.PlayTracks(AudioPlayerMode.Playlist);
                        }
                        break;
                    case AudioPlayerState.Paused:
                        PlayerManager.Play();
                        break;
                    case AudioPlayerState.Playing:
                        PlayerManager.Pause();
                        break;
                }
            }
            catch (Exception exception)
            {
                var dialogResult = ResourceService.GetString("Dialog_Result_Ok");
                await _pageDialogService.DisplayAlertAsync("", exception.Message, dialogResult);
            }
        }
        private bool CanPlayNext()
        {
            return PlayerManager.CanPlayNextTrack();
        }

        private void PlayNext()
        {
            PlayerManager.PlayNextTrack();
        }
        
        private bool CanPause()
        {
            return PlayerManager.AudioPlayerState == AudioPlayerState.Playing;
        }

        private void Pause()
        {
            PlayerManager.Pause();
        }

    }
}
