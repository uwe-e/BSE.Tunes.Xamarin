﻿using System.Net.Http.Headers;

namespace System.Net.Http
{
    public static class HttpClientExtensions
    {
        public static void SetToken(this HttpClient client, string scheme, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
        }

        public static void SetBearerToken(this HttpClient client, string token)
        {
            client.SetToken("Bearer", token);
        }
    }
}
