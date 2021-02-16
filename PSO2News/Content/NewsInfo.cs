using System;
using Newtonsoft.Json;

namespace PSO2News.Content
{
    public class NewsInfo
    {
        [JsonProperty("type")]
        public NewsType Type { get; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; }

        [JsonProperty("title")]
        public string Title { get; }

        [JsonProperty("url")]
        public string Url { get; }

        public NewsInfo(NewsType type, DateTime timestamp, string title, string url)
        {
            Type = type;
            Timestamp = timestamp;
            Title = title;
            Url = url;
        }
    }
}