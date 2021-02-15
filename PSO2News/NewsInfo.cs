using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PSO2News
{
    public class NewsInfo
    {
        [JsonProperty("id")]
        public int Id { get; }

        [JsonProperty("type")]
        public NewsType Type { get; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; }

        [JsonProperty("title")]
        public string Title { get; }

        [JsonProperty("url")]
        public string Url { get; }

        private readonly HttpClient _http;

        public NewsInfo(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> GetNewsBodyAsync()
        {
            var page = await _http.GetStringAsync(new Uri(Url));
            return page;
        }
    }
}