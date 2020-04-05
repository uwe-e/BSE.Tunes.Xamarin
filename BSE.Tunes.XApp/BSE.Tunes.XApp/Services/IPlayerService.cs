using BSE.Tunes.XApp.Models.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public interface IPlayerService
    {
        Task SetTrackAsync(Track track);
        void Play();
        void Pause();
        void PlayNext();
    }
}
