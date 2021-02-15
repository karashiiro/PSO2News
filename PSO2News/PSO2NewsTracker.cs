using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace PSO2News
{
    public class PSO2NewsTracker : IDisposable
    {
        private const string NewsUrl = "http://pso2.jp/players/news/maintenance/";

        private readonly HttpClient _http;

        public PSO2NewsTracker()
        {
            _http = new HttpClient();
        }

        public IEnumerable<NewsInfo> GetNews(DateTime after = default, NewsType type = NewsType.Any)
        {
            return new NewsInfoEnumerable()
                .Where(ni => ni.Timestamp > after)
                .Where(ni => type == NewsType.Any || ni.Type == type);
        }

        public void Dispose()
        {
            _http?.Dispose();
        }

        private class NewsInfoEnumerable : IEnumerable<NewsInfo>
        {
            public IEnumerator<NewsInfo> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
