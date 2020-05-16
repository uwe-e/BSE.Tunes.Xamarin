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
        private readonly IEventAggregator _eventAggregator;
        private IPlayerService _playerService { get; }

        public NavigableCollection<int> Playlist { get; set; }
        public AudioPlayerMode AudioPlayerMode { get; private set; }
        public Track CurrentTrack { get; private set; }
        public AudioPlayerState AudioPlayerState { get; private set; } = AudioPlayerState.Closed;
        public float Progress => _playerService?.Progress ?? default;

        public PlayerManager(IDataService dataService,
            IPlayerService playerService,
            IEventAggregator eventAggregator)
        {
            _dataService = dataService;
            _playerService = playerService;
            _eventAggregator = eventAggregator;
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
            if (trackId > 0)
            {
                Track track = await _dataService.GetTrackById(trackId);
                if (track != null)
                {
                    _playerService.SetTrack(track);
                }
            }
        }

        public void PlayTracks(ObservableCollection<int> trackIds, AudioPlayerMode audioPlayerMode)
        {
            Playlist = trackIds.ToNavigableCollection();
            PlayTracks(AudioPlayerMode);
        }

        public bool CanPlay()
        {
            return Playlist?.Count > 0;
        }

        public bool CanPlayNextTrack()
        {
            return Playlist?.CanMoveNext ?? false;
        }

        public async void PlayNextTrack()
        {
            if (CanPlayNextTrack())
            {
                if (Playlist.MoveNext())
                {
                    _playerService.Stop();
                    var trackId = Playlist.Current;
                    if (trackId > 0)
                    {
                        Track track = await _dataService.GetTrackById(trackId);
                        if (track != null)
                        {
                            Console.WriteLine($"Next Track with ID {track.Id} requested: ");
                            _playerService.SetTrack(track);
                        }
                    }
                }
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
                    var trackId = Playlist.Current;
                    if (trackId > 0)
                    {
                        CurrentTrack = await _dataService.GetTrackById(trackId);
                    }
                    break;
                case MediaState.Ended:
                    if (AudioPlayerMode != AudioPlayerMode.None && AudioPlayerMode != AudioPlayerMode.Song && CanPlayNextTrack())
                    {
                        PlayNextTrack();
                    }
                    break;
            }
            _eventAggregator.GetEvent<MediaStateChangedEvent>().Publish(mediaState);
        }
    }
}
