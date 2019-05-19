using BSE.Tunes.XApp.Models.Contract;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public interface IDataService
    {
        Task<bool> IsEndPointAccessibleAsync();
        Task<bool> IsEndPointAccessibleAsync(string serviceEndPoint);
        Task<ObservableCollection<Album>> GetFeaturedAlbums(int limit);
    }
}
