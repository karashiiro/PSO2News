using System.Net.Http;

namespace PSO2News
{
    public class NewsInfoHydrator
    {
        private readonly HttpClient _http;

        public NewsInfoHydrator(HttpClient http)
        {
            _http = http;
        }

        public RichNewsInfo HydrateNewsInfo(NewsInfo newsInfo)
        {
            return new RichNewsInfo(newsInfo, _http);
        }
    }
}