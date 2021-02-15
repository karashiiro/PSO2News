using System;
using System.Net.Http;
using PSO2News.StorageBackends;

namespace PSO2News
{
    public class PSO2NewsTracker : IDisposable
    {
        private const string NewsUrl = "http://pso2.jp/players/news/maintenance/";

        private readonly HttpClient _http;

        private readonly Store _store;

        public PSO2NewsTracker(Store storageBackend)
        {
            _http = new HttpClient();

            _store = storageBackend;
        }

        public void Dispose()
        {
            _store?.Dispose();
            _http?.Dispose();
        }
    }
}
