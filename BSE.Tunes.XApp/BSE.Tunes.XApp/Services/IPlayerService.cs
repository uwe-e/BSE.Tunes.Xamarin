using BSE.Tunes.XApp.Models.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public interface IPlayerService
    {
        AudioPlayerState AudioPlayerState { get; }
        float Progress { get; }
        event Action<AudioPlayerState> AudioPlayerStateChanged;
        event Action<MediaState> MediaStateChanged;
        void SetTrack(Track track);
        void Play();
        void Pause();
        void Stop();
    }
}
