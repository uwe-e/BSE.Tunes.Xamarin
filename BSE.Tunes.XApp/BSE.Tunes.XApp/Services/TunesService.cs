using BSE.Tunes.XApp.Extensions;
using System;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public class TunesService : ITunesService
    {
        private readonly IRequestService requestService;
        private readonly ISettingsService settingsService;

        public TunesService(IRequestService requestService, ISettingsService settingsService)
        {
            this.requestService = requestService;
            this.settingsService = settingsService;
        }

        public Task<bool> IsEndPointAccessibleAsync()
        {
            return IsEndPointAccessibleAsync(this.settingsService.ServiceEndPoint);
        }

        public Task<bool> IsEndPointAccessibleAsync(string serviceEndPoint)
        {
            var builder = new UriBuilder(serviceEndPoint);
            builder.AppendToPath("api/tunes/IsHostAccessible");
            //try
            //{
            return this.requestService.GetAsync<bool>(builder.Uri);
            //}
            //catch (Exception)
            //{
            //    throw new ConnectivityException();
            //}
        }
    }
}
