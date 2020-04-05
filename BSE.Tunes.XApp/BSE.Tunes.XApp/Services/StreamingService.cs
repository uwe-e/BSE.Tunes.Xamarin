using BSE.Tunes.XApp.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public class StreamingService : IStreamingService
    {
        private IRequestService _requestService;
        private ISettingsService _settingsService;

        public StreamingService(IRequestService requestService, ISettingsService settingsService)
        {
            _requestService = requestService;
            _settingsService = settingsService;
        }

        public async Task<long> GetContentLength(Guid guid)
        {
            long contentLength = -1;
            Uri requestUri = GetRequestUri(guid);

            using (var httpClient = await _requestService.GetHttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, requestUri))
                {
                    using (HttpResponseMessage responseMessage = await httpClient.SendAsync(request))
                    {
                        contentLength = responseMessage.Content.Headers.ContentLength.GetValueOrDefault(0);
                    }
                }
            }
            return contentLength;
        }

        public async Task<int> GetPartialContent(Guid guid, long rangeFrom, long rangeTo)
        {
            int partialContent = -1;
            Uri requestUri = GetRequestUri(guid);

            using (var httpClient = await _requestService.GetHttpClient(false))
            {
                httpClient.AddRange(rangeFrom, rangeTo);
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                {
                    using (var responseMessage = await httpClient.SendAsync(request))
                    {
                        partialContent = (int)responseMessage.Content.Headers.ContentLength.GetValueOrDefault(0);
                    }
                }
            }
            return partialContent;
        }

        private Uri GetRequestUri(Guid guid)
        {
            var builder = new UriBuilder(_settingsService.ServiceEndPoint);
            builder.AppendToPath($"/api/files/audio/{guid}");
            return builder.Uri;
        }
    }
}
