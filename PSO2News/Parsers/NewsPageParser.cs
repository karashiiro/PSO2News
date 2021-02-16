using System;
using System.Collections.Generic;
using System.Threading;
using PSO2News.Content;

namespace PSO2News.Parsers
{
    public abstract class NewsPageParser
    {
        public abstract IAsyncEnumerable<NewsInfo> GetNews(
            DateTime after,
            CancellationToken token);
    }
}