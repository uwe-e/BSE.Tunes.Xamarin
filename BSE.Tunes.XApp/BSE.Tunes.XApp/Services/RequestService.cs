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
        public async Task<TResult> GetAsync<TResult>(Uri uri)
        {
            TResult result = default(TResult);
            using (var client = CreateHttpClient())
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
            using (var client = CreateHttpClient())
            {
                var serialized = await Task.Run(() => JsonConvert.SerializeObject(from));

                //var serialized = await responseMessage.Content.ReadAsStringAsync();
            }
            return result;
        }

        private HttpClient CreateHttpClient(string token = "")
        {
            //if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            //{
            //    throw new ConnectivityException();
            //}
            var httpClient = new HttpClient();
            return httpClient;
        }
    }
}
