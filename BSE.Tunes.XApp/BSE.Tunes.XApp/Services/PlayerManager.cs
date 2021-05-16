using BSE.Tunes.XApp.Collections;
using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Models.Contract;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public class PlayerManager : IPlayerManager
    {
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ITimerService _timerService;
        private readonly IPlayerService _playerService;
        private float _oldProgress;
        
        private int _attempToPlay;

        public NavigableCollection<int> Playlist { get; set; }
        public AudioPlayerMode AudioPlayerMode { get; private set; }
        public Track CurrentTrack { get; private set; }
        public AudioPlayerState AudioPlayerState { get; private set; } = AudioPlayerState.Closed;
        public float Progress => _playerService?.Progress ?? default;

        public PlayerManager(IDataService dataService,
            IPlayerService playerService,
            ISettingsService settingsService,
            IEventAggregator eventAggregator,
            ITimerService timerService)
        {
            _dataService = dataService;
            _playerService = playerService;
            _settingsService = settingsService;
            _eventAggregator = eventAggregator;
            _timerService = timerService;
            _timerService.TimerElapsed += OnTimerElapsed;
            _timerService.Start();
            _playerService.AudioPlayerStateChanged += OnAudioPlayerStateChanged;
            _playerService.MediaStateChanged += OnMediaStateChanged;
        }

        public void Pause()
        {
            _playerService.Pause();
        }

        public void Play()
        {
            _playerService.Play();
        }
        public async void PlayTracks(AudioPlayerMode audioPlayerMode)
        {
            AudioPlayerMode = audioPlayerMode;
            int trackId = Playlist?.FirstOrDefault() ?? 0;
            await PlayTrack(trackId);
        }

        public void PlayTracks(ObservableCollection<int> trackIds, AudioPlayerMode audioPlayerMode)
        {
            _playerService.Stop();
            Playlist = trackIds.ToNavigableCollection();
            PlayTracks(audioPlayerMode);
        }

        public bool CanPlay()
        {
            return Playlist?.Count > 0;
        }

        public bool CanPlayPreviosTrack()
        {
            return Playlist?.CanMovePrevious ?? false;
        }

        public bool CanPlayNextTrack()
        {
            return Playlist?.CanMoveNext ?? false;
        }

        public async void PlayPreviousTrack()
        {
            if (CanPlayPreviosTrack())
            {
                if (Playlist.MovePrevious())
                {
                    await PlayTrack(Playlist.Current);
                }
            }
        }

        public async void PlayNextTrack()
        {
            if (CanPlayNextTrack())
            {
                if (Playlist.MoveNext())
                {
                    await PlayTrack(Playlist.Current);
                }
            }
        }

        private async Task PlayTrack(int trackId)
        {
            _playerService.Stop();
            if (trackId > 0)
            {
                Track track = await _dataService.GetTrackById(trackId);
                if (track != null)
                {
                    _playerService.SetTrack(track, _dataService.GetImage(track.Album.AlbumId, true));
                }
            }
        }
        
        public Task<bool> CloseAsync()
        {
            return _playerService.CloseAsync();
        }
        
        private void OnTimerElapsed()
        {
            var newProgress = Progress;
            Console.WriteLine($"AudioPlayerState: {AudioPlayerState}");
            if (newProgress != _oldProgress)
            {
                _eventAggregator.GetEvent<MediaProgressChangedEvent>().Publish(new MediaProgress
                {
                    Progress = newProgress,
                    ProgressOld = _oldProgress
                });
                _oldProgress = newProgress;
            }
        }

        private void OnAudioPlayerStateChanged(AudioPlayerState audioPlayerState)
        {
            if (AudioPlayerState != audioPlayerState)
            {
                AudioPlayerState = audioPlayerState;
                _eventAggregator.GetEvent<AudioPlayerStateChangedEvent>().Publish(audioPlayerState);
            }
        }

        private async void OnMediaStateChanged(MediaState mediaState)
        {
            switch (mediaState)
            {
                case MediaState.Opened:
                    _attempToPlay = 0;
                    var trackId = Playlist.Current;
                    if (trackId > 0)
                    {
                        CurrentTrack = await _dataService.GetTrackById(trackId);
                        UpdateHistory(CurrentTrack);
                    }
                    break;
                case MediaState.Ended:
                    if (AudioPlayerMode != AudioPlayerMode.None && AudioPlayerMode != AudioPlayerMode.Song && CanPlayNextTrack())
                    {
                        PlayNextTrack();
                    }
                    break;
                case MediaState.BadRequest:
                    Console.WriteLine($"{nameof(OnMediaStateChanged)} bad request");
                    if (AudioPlayerMode != AudioPlayerMode.None && AudioPlayerMode != AudioPlayerMode.Song && CanPlayNextTrack())
                    {
                        //If we have a bad request, we try to play the track another time;
                        TryToPlayTrack();
                        
                    }
                    break;
            }
            _eventAggregator.GetEvent<MediaStateChangedEvent>().Publish(mediaState);
        }

        private void TryToPlayTrack()
        {
            if (_attempToPlay < 3)
            {
                _attempToPlay += 1;
                _playerService.SetTrack(CurrentTrack);
            }
        }

        private async void UpdateHistory(Track track)
        {
            var userName = _settingsService.User.UserName;
            if (!string.IsNullOrEmpty(userName))
            {
                await _dataService.UpdateHistory(new History
                {
                    PlayMode = (int)AudioPlayerMode,
                    AlbumId = track.Album.Id,
                    TrackId = track.Id,
                    UserName = userName,
                    PlayedAt = DateTime.Now
                });
            }
        }
    }
}
