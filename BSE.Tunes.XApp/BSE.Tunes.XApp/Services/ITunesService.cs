using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public interface ITunesService
    {
        Task<bool> IsEndPointAccessibleAsync();
        Task<bool> IsEndPointAccessibleAsync(string serviceEndPoint);
    }
}
