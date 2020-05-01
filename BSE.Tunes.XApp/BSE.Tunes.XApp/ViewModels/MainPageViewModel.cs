using BSE.Tunes.XApp.Collections;
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
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IPlayerManager _playerManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IPageDialogService _pageDialogService;
        private Track _currentTrack;
        private ICommand _playCommand;
        private ICommand _pauseCommand;
        private ICommand _playNextCommand;
        private NavigableCollection<int> _playlist;
        private AudioPlayerState _audioPlayerState = AudioPlayerState.Closed;
        private AudioPlayerMode _audioPlayerMode = AudioPlayerMode.None;

        public ICommand PlayCommand => _playCommand
            ?? (_playCommand = new DelegateCommand(Play));
        public ICommand PauseCommand => _pauseCommand
            ?? (_pauseCommand = new DelegateCommand(Pause, CanPause));
        public ICommand PlayNextCommand => _playNextCommand
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

        public MainPageViewModel(INavigationService navigationService,
            IResourceService resourceService,
            IPlayerManager playerManager,
            IEventAggregator eventAggregator,
            IPageDialogService pageDialogService)
            : base(navigationService, resourceService)
        {
            _playerManager = playerManager;
            _pageDialogService = pageDialogService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AudioPlayerStateChangedEvent>().Subscribe((args) => {
                if (args is AudioPlayerState audioPlayerState)
                {
                    if (AudioPlayerState != audioPlayerState)
                    {
                        AudioPlayerState = audioPlayerState;
                    }
                };
            }, ThreadOption.UIThread);
        }

        //private bool CanPlay()
        //{
        //    return true;
        //}

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
            if (CanPause())
            {
                _playerManager.Pause();
            }
        }
        
        private bool CanPlayNext()
        {
            return _playerManager.CanPlayNextTrack();
        }

        private void PlayNext()
        {
            _playerManager.PlayNextTrack();
        }
    }
}
