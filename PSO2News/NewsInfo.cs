using System;
using Newtonsoft.Json;

namespace PSO2News
{
    public class NewsInfo
    {
        [JsonProperty("id")]
        public int Id { get; protected set; }

        [JsonProperty("type")]
        public NewsType Type { get; protected set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; protected set; }

        [JsonProperty("title")]
        public string Title { get; protected set; }

        [JsonProperty("url")]
        public string Url { get; protected set; }
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