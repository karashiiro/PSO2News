using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace PSO2News
{
    public class PSO2NewsTracker : IDisposable
    {
        private const string NewsUrl = "http://pso2.jp/players/news/maintenance/";
        private static readonly Regex TimeRegex = new Regex(@"(?<year>\d{4})\/(?<month>\d{2})\/(?<day>\d{2}) (?<hour>\d{2}):(?<minute>\d{2})", RegexOptions.Compiled);

        private readonly HttpClient _http;

        public PSO2NewsTracker()
        {
            _http = new HttpClient();
        }

        public async IAsyncEnumerable<NewsInfo> GetNews(
            DateTime after = default,
            NewsType type = NewsType.Any,
            [EnumeratorCancellation] CancellationToken token = default)
        {
            do
            {
                var web = new HtmlWeb();
                var page = await web.LoadFromWebAsync(NewsUrl, token);

                var list = page.DocumentNode.SelectSingleNode("//section[@class='topic--list']/ul");

                foreach (var newsInfo in list.SelectNodes("/li"))
                {
                    var linkNode = newsInfo.SelectSingleNode("/a");
                    var url = linkNode.GetAttributeValue("href", "");

                    var typeNode = linkNode.SelectSingleNode("/span[1]");
                    var newsType = GetNewsType(typeNode.InnerText);
                    if (type != NewsType.Any && newsType != type)
                    {
                        continue;
                    }

                    var titleNode = linkNode.SelectSingleNode("/span[2]");
                    var title = titleNode.InnerText;

                    var timeNode = linkNode.SelectSingleNode("/span[3]/time");
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
                        continue;
                    }

                    yield return new NewsInfo(_http, 0, newsType, parsedTime, title, url);
                }
            } while (!token.IsCancellationRequested);
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

        public void Dispose()
        {
            _http?.Dispose();
        }
    }
}
