using BSE.Tunes.XApp.Extensions;
using BSE.Tunes.XApp.Models.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public class DataService : IDataService
    {
        private readonly IRequestService requestService;
        private readonly ISettingsService settingsService;

        public DataService(IRequestService requestService, ISettingsService settingsService)
        {
            this.requestService = requestService;
            this.settingsService = settingsService;
        }

        public Task<bool> IsEndPointAccessibleAsync()
        {
            return IsEndPointAccessibleAsync(this.settingsService.ServiceEndPoint);
        }

        public async Task<bool> IsEndPointAccessibleAsync(string serviceEndPoint)
        {
            bool isAccessible = false;
            var builder = new UriBuilder(serviceEndPoint);
            builder.AppendToPath("api/tunes/IsHostAccessible");

            using (var client = await this.requestService.GetHttpClient(false))
            {
                try
                {
                    var responseMessage = await client.GetAsync(builder.Uri);
                    var serialized = await responseMessage.Content.ReadAsStringAsync();
                    isAccessible = await Task.Run(() => JsonConvert.DeserializeObject<bool>(serialized));
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return isAccessible;
        }

        public Task<ObservableCollection<Album>> GetFeaturedAlbums(int limit)
        {
            string strUrl = string.Format("{0}/api/v2/albums/featured?limit={1}", this.settingsService.ServiceEndPoint, limit);
            return this.requestService.GetAsync<ObservableCollection<Album>>(new UriBuilder(strUrl).Uri);
        }

        public Task<ObservableCollection<Album>> GetNewestAlbums(int limit)
        {
            string strUrl = string.Format("{0}/api/v2/albums/newest?limit={0}", this.settingsService.ServiceEndPoint, limit);
            return this.requestService.GetAsync<ObservableCollection<Album>>(new UriBuilder(strUrl).Uri);
        }

        public Uri GetImage(Guid imageId, bool asThumbnail = false)
        {
            var builder = new UriBuilder(this.settingsService.ServiceEndPoint);
            builder.AppendToPath(string.Format("/api/files/image/{0}", imageId.ToString()));

            return builder.Uri;
        }

        public Task<Album> GetAlbumById(int albumId)
        {
            string strUrl = $"{this.settingsService.ServiceEndPoint}/api/v2/albums/{albumId}";
            return this.requestService.GetAsync<Album>(new UriBuilder(strUrl).Uri);
        }

        public Task<int> GetNumberOfAlbumsByGenre(int? genreId)
        {
            string strUrl = $"{this.settingsService.ServiceEndPoint}/api/v2/albums/genre/{genreId ?? 0}/count";
            return this.requestService.GetAsync<int>(new UriBuilder(strUrl).Uri);
        }

        public Task<ObservableCollection<Album>> GetAlbumsByGenre(int? genreId, int skip, int limit)
        {
            string strUrl = $"{this.settingsService.ServiceEndPoint}/api/v2/albums/genre/{genreId ?? 0}/?skip={skip}&limit={limit}";
            return this.requestService.GetAsync<ObservableCollection<Album>>(new UriBuilder(strUrl).Uri);
        }
        
        public Task<SystemInfo> GetSystemInfo()
        {
             string strUrl = $"{this.settingsService.ServiceEndPoint}/api/system";
            return this.requestService.GetAsync<SystemInfo>(new UriBuilder(strUrl).Uri);
        }

        public Task<Track> GetTrackById(int trackId)
        {
            string strUrl = $"{this.settingsService.ServiceEndPoint}/api/v2/tracks/{trackId}";
            return this.requestService.GetAsync<Track>(new UriBuilder(strUrl).Uri);
        }

        public Task<ObservableCollection<int>> GetTrackIdsByGenre(int? genreId = null)
        {
            string strUrl = $"{this.settingsService.ServiceEndPoint}/api/v2/tracks/genre/{genreId ?? 0}";
            return this.requestService.GetAsync<ObservableCollection<int>>(new UriBuilder(strUrl).Uri);
        }

        
    }
}
