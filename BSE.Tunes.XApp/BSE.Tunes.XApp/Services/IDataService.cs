using BSE.Tunes.XApp.Models.Contract;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public interface IDataService
    {
        Task<Album> GetAlbumById(int albumId);
        Task<ObservableCollection<Album>> GetFeaturedAlbums(int limit);
        Task<ObservableCollection<Album>> GetNewestAlbums(int limit);
        Task<int> GetNumberOfAlbumsByGenre(int? genreId);
        Task<ObservableCollection<Album>> GetAlbumsByGenre(int? genreId, int skip, int limit);
        Uri GetImage(Guid imageId, bool asThumbnail = false);
        Task<string[]> GetSearchSuggestions(string searchPhrase);
        Task<SystemInfo> GetSystemInfo();
        Task<Track> GetTrackById(int trackId);
        Task<ObservableCollection<int>> GetTrackIdsByGenre(int? genreId = null);
        Task<bool> IsEndPointAccessibleAsync();
        Task<bool> IsEndPointAccessibleAsync(string serviceEndPoint);
        Task<bool> UpdateHistory(History history);
    }
}
