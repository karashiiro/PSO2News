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
        public string Url { get; set; }

        private readonly HttpClient _http;

        internal NewsInfo(HttpClient http, int id, NewsType type, DateTime timestamp, string title, string url)
        {
            _http = http;

            Id = id;
            Type = type;
            Timestamp = timestamp;
            Title = title;
            Url = url;
        }

        /// <summary>
        /// Requests the body text of the news post.
        /// </summary>
        public async Task<string> GetNewsTextAsync()
        {
            var page = await _http.GetStringAsync(new Uri(Url));
            return page;
        }
    }

    public enum NewsType
    {
        None,
        Notice,
        Maintenance,
        Update,
        Event,
        Campaign,
        Media,
    }
}