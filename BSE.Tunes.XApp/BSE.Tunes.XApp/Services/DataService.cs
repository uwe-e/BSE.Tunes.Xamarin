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

        public Task<ObservableCollection<Album>> GetFeaturedAlbums(int limit)
        {
            //string strUrl = string.Format("{0}/api/v2/albums/featured?limit={1}", ServiceUrl, limit);
            //return await GetHttpResponse<ObservableCollection<Album>>(new Uri(strUrl));
            var builder = new UriBuilder(this.settingsService.ServiceEndPoint);
            builder.AppendToPath(string.Format("/api/v2/albums/featured?limit={0}", limit));

            string strUrl = string.Format("{0}/api/v2/albums/featured?limit={1}", this.settingsService.ServiceEndPoint, limit);

            return this.requestService.GetAsync<ObservableCollection<Album>>(new UriBuilder(strUrl).Uri);
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
                catch (Exception)
                {
                    throw;
                }
            }

            return isAccessible;
        }
    }
}
