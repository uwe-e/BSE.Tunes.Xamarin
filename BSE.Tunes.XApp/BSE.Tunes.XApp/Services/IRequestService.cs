﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public interface IRequestService
    {
        Task<T> GetAsync<T>(Uri uri);
        Task<U> PostAsync<U, T>(Uri uri, T from);
        Task<U> PutAsync<U, T>(Uri uri, T from);
        Task DeleteAsync(Uri uri);
        Task<HttpClient> GetHttpClient(bool withRefreshToken = true);
    }
}
