using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace VideoInsights.Web.Helpers
{
    public static class HttpClientHelper
    {
        public static HttpClient CreateInstance(string url)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}