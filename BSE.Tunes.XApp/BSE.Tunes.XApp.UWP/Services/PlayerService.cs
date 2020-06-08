using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.UWP.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlayerService))]
namespace BSE.Tunes.XApp.UWP.Services
{
    public class PlayerService : IPlayerService
    {
        public AudioPlayerState AudioPlayerState => throw new NotImplementedException();

        public float Progress => throw new NotImplementedException();

        public event Action<AudioPlayerState> AudioPlayerStateChanged;
        public event Action<MediaState> MediaStateChanged;

        public Task<bool> CloseAsync()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Play()
        {
            throw new NotImplementedException();
        }

        public void SetTrack(Track track)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
