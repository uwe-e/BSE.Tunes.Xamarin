using BSE.Tunes.XApp.iOS.Services;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlayerService))]
namespace BSE.Tunes.XApp.iOS.Services
{
    public class PlayerService : IPlayerService
    {
        IDataService _dataService => DependencyService.Resolve<IDataService>();

        public void Pause()
        {
        }

        public void Play()
        {
        }

        public void PlayNext()
        {
        }

        public Task SetTrackAsync(Track track)
        {
            if (track != null)
            {

            }
            return Task.CompletedTask;
        }
    }
}