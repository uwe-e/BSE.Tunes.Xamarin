﻿using BSE.Tunes.XApp.Collections;
using BSE.Tunes.XApp.Models.Contract;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public interface IPlayerManager
    {
        NavigableCollection<int> Playlist { get; set; }
        AudioPlayerMode AudioPlayerMode { get; }
        AudioPlayerState AudioPlayerState { get; }
        Track CurrentTrack { get; }
        float Progress { get; }
        bool CanPlay();
        bool CanPlayPreviosTrack();
        bool CanPlayNextTrack();
        void Play();
        void PlayTracks(AudioPlayerMode audioPlayerMode);
        void PlayTracks(ObservableCollection<int> trackIds, AudioPlayerMode audioPlayerMode);
        void PlayPreviousTrack();
        void PlayNextTrack();
        void Pause();
        Task<bool> CloseAsync();
    }
}
