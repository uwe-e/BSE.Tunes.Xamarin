using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace BSE.Tunes.XApp.Services
{
    public class RequestService : IRequestService
    {
        private readonly ISettingsService settingsService;
        private readonly IAuthenticationService authenticationService;

        public RequestService(ISettingsService settingsService, IAuthenticationService authenticationService)
        {
            this.settingsService = settingsService;
            this.authenticationService = authenticationService;
        }

        public async Task<TResult> GetAsync<TResult>(Uri uri)
        {
            TResult result = default(TResult);
            using (var client = await GetHttpClient())
            {
                try
                {
                    var responseMessage = await client.GetAsync(uri);
                    //responseMessage.EnsureExtendedSuccessStatusCode();
                    var serialized =  await responseMessage.Content.ReadAsStringAsync();
                    result = await Task.Run(() => JsonConvert.DeserializeObject<TResult>(serialized));
                }
                catch (Exception)
                {
                    throw;
                }
            }
                return result;
        }

        public async Task<TResult> PostAsync<TRequest, TResult>(Uri uri, TResult from)
        {
            TResult result = default(TResult);
            using (var client = GetHttpClient())
            {
                var serialized = await Task.Run(() => JsonConvert.SerializeObject(from));

                //var serialized = await responseMessage.Content.ReadAsStringAsync();
            }
            return result;
        }

        public async Task<HttpClient> GetHttpClient(bool withRefreshToken = true)
        {
            //if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            //{
            //    throw new ConnectivityException();
            //}
            var httpClient = new HttpClient();
            var tokenResponse = this.authenticationService.TokenResponse;
            if (withRefreshToken)
            {
                tokenResponse = await this.authenticationService.RequestRefreshTokenAsync(tokenResponse.RefreshToken);
                httpClient.SetBearerToken(tokenResponse.AccessToken);
            }
            return httpClient;
        }
    }
}
