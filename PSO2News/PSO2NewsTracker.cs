using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace PSO2News
{
    public class PSO2NewsTracker : IDisposable
    {
        private const string NewsUrl = "http://pso2.jp/players/news/?page={0}";
        private static readonly Regex TimeRegex = new Regex(@"(?<year>\d{4})\/(?<month>\d{2})\/(?<day>\d{2}) (?<hour>\d{2}):(?<minute>\d{2})", RegexOptions.Compiled);

        private readonly HttpClient _http;
        private readonly bool _externalHttpClient;

        public PSO2NewsTracker()
        {
            _http = new HttpClient();
        }

        public PSO2NewsTracker(HttpClient http)
        {
            _http = http;
            _externalHttpClient = true;
        }

        public async IAsyncEnumerable<NewsInfo> GetNews(
            DateTime after = default,
            [EnumeratorCancellation] CancellationToken token = default)
        {
            var web = new HtmlWeb();
            var page = await web.LoadFromWebAsync(string.Format(NewsUrl, 1), token);

            var list = page.DocumentNode.SelectSingleNode("//section[@class='topic--list']/ul");

            foreach (var newsInfo in list.SelectNodes("li"))
            {
                var linkNode = newsInfo.SelectSingleNode("a");
                var url = new Uri(linkNode.GetAttributeValue("href", ""));

                var typeNode = linkNode.SelectSingleNode("span[1]");
                var newsType = GetNewsType(typeNode.InnerText);

                var titleNode = linkNode.SelectSingleNode("span[2]");
                var title = titleNode.InnerText;

                var timeNode = linkNode.SelectSingleNode("span[3]/time");
                var timeParts = TimeRegex.Match(timeNode.InnerText).Groups;
                var parsedTime = new DateTime(
                    int.Parse(timeParts["year"].Value),
                    int.Parse(timeParts["month"].Value),
                    int.Parse(timeParts["day"].Value),
                    int.Parse(timeParts["hour"].Value),
                    int.Parse(timeParts["minute"].Value),
                    0);
                if (parsedTime <= after)
                {
                    break;
                }
                
                yield return new NewsInfo(_http, newsType, parsedTime, title, url.ToString());
            }
        }

        private static NewsType GetNewsType(string typeName)
        {
            return typeName switch
            {
                "お知らせ" => NewsType.Notice,
                "復旧" => NewsType.Recovery,
                "重要" => NewsType.Important,
                "メンテナンス" => NewsType.Maintenance,
                "アップデート" => NewsType.Update,
                "イベント" => NewsType.Event,
                "キャンペーン" => NewsType.Campaign,
                "メディア" => NewsType.Media,
                _ => NewsType.Unknown,
            };
        }

        private bool _disposed;
        public void Dispose()
        {
            Dispose(_disposed);
            _disposed = true;
        }

        private void Dispose(bool disposed)
        {
            if (disposed) return;

            if (_externalHttpClient)
            {
                _http?.Dispose();
            }
        }
    }
}
