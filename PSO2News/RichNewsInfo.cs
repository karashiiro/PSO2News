using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PSO2News
{
    public class RichNewsInfo : NewsInfo
    {
        private readonly HttpClient _http;

        public RichNewsInfo(NewsInfo ni, HttpClient http)
        {
            _http = http;

            Id = ni.Id;
            Type = ni.Type;
            Timestamp = ni.Timestamp;
            Title = ni.Title;
            Url = ni.Url;
        }

        public async Task<string> GetNewsBodyAsync()
        {
            var page = await _http.GetStringAsync(new Uri(Url));
            return page;
        }
    }
}