using System;
using System.Collections.Generic;
using System.Threading;
using PSO2News.Content;
using PSO2News.Parsers;

namespace PSO2News
{
    public class PSO2NewsTracker
    {
        private readonly NewsPageParser _pageParser;

        public PSO2NewsTracker(NewsSource source)
        {
            _pageParser = source switch
            {
                NewsSource.PSO2 => new PSO2PageParser(),
                NewsSource.NGS => new NGSPageParser(),
                _ => throw new NotImplementedException(),
            };
        }

        public IAsyncEnumerable<NewsInfo> GetNews(
            DateTime after = default,
            CancellationToken token = default)
        {
            return _pageParser.GetNews(after, token);
        }
    }
}
