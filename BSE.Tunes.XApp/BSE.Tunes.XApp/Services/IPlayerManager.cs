using BSE.Tunes.XApp.Collections;
using BSE.Tunes.XApp.Models.Contract;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public interface IPlayerManager
    {
        NavigableCollection<int> Playlist { get; set; }
        AudioPlayerMode AudioPlayerMode { get; }
        Track CurrentTrack { get; }
        bool CanPlay();
        bool CanPlayNextTrack();
        void Play();
        void PlayTracks(AudioPlayerMode audioPlayerMode);
        void PlayTracks(ObservableCollection<int> trackIds, AudioPlayerMode audioPlayerMode);
        void PlayNextTrack();
        void Pause();
        Task SetTrackAsync(Track track);
    }
}
