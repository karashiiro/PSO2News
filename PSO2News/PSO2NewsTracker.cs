using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using PSO2News.Content;

namespace PSO2News
{
    public static class PSO2NewsTracker
    {
        private const string NewsUrl = "http://pso2.jp/players/news/";
        private static readonly Regex TimeRegex = new Regex(@"(?<year>\d{4})\/(?<month>\d{2})\/(?<day>\d{2}) (?<hour>\d{2}):(?<minute>\d{2})", RegexOptions.Compiled);

        public static async IAsyncEnumerable<NewsInfo> GetNews(
            DateTime after = default,
            [EnumeratorCancellation] CancellationToken token = default)
        {
            var curUrl = NewsUrl;
            do
            {
                var web = new HtmlWeb();
                var page = await web.LoadFromWebAsync(curUrl, token);

                var nextButton = page.DocumentNode.SelectSingleNode("//li[@class='pager--next']/a");
                curUrl = nextButton?.GetAttributeValue("href", null);

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
                        yield break;
                    }

                    yield return newsType switch
                    {
                        NewsType.Maintenance => await new MaintenanceNewsInfo(newsType, parsedTime, title, url.ToString()).Initialize(token),
                        _ => new NewsInfo(newsType, parsedTime, title, url.ToString()),
                    };
                }
            } while (curUrl != null);
        }

        private static NewsType GetNewsType(string typeName)
        {
            return typeName switch
            {
                "お知らせ" => NewsType.Notice,
                "復旧" => NewsType.Recovery,
                "重要" => NewsType.Important,
                "FAQ" => NewsType.FAQ,
                "WEB" => NewsType.Web,
                "対応状況" => NewsType.Bugs,
                "メンテナンス" => NewsType.Maintenance,
                "アップデート" => NewsType.Update,
                "イベント" => NewsType.Event,
                "キャンペーン" => NewsType.Campaign,
                "メディア" => NewsType.Media,
                _ => NewsType.Unknown,
            };
        }
    }
}
