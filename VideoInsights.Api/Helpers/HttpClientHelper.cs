﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace VideoInsights.Api.Helpers
{
    public static class HttpClientHelper
    {
        public static HttpClient CreateInstance(string url, string key)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}