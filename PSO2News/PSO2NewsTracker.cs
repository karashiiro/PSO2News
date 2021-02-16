using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using PSO2News.Content;
using PSO2News.SiteInfo;

namespace PSO2News
{
    public class PSO2NewsTracker
    {
        private static readonly Regex TimeRegex = new Regex(@"(?<year>\d{4})\/(?<month>\d{2})\/(?<day>\d{2}) (?<hour>\d{2}):(?<minute>\d{2})", RegexOptions.Compiled);

        private readonly NewsSiteInfo _siteInfo;

        public PSO2NewsTracker(NewsSource source)
        {
            _siteInfo = source switch
            {
                NewsSource.PSO2 => new PSO2SiteInfo(),
                NewsSource.NGS => new NGSSiteInfo(),
                _ => throw new NotImplementedException(),
            };
        }

        public async IAsyncEnumerable<NewsInfo> GetNews(
            DateTime after = default,
            [EnumeratorCancellation] CancellationToken token = default)
        {
            var curUrl = _siteInfo.BaseUrl;
            do
            {
                var web = new HtmlWeb();
                var page = await web.LoadFromWebAsync(curUrl, token);

                var nextButton = page.DocumentNode.SelectSingleNode(_siteInfo.NextButtonSelector);
                curUrl = nextButton?.GetAttributeValue("href", null);

                var list = page.DocumentNode.SelectSingleNode(_siteInfo.UlSelector);

                foreach (var newsInfo in list.SelectNodes("li"))
                {
                    var linkNode = newsInfo.SelectSingleNode(_siteInfo.LinkSelector);
                    var url = linkNode.GetAttributeValue("href", "");

                    var typeNode = linkNode.SelectSingleNode(_siteInfo.TypeSelector);
                    var newsType = GetNewsType(typeNode.InnerText);

                    var titleNode = linkNode.SelectSingleNode(_siteInfo.TitleSelector);
                    var title = titleNode.InnerText;

                    var timeNode = linkNode.SelectSingleNode(_siteInfo.TimeSelector);
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
                        NewsType.Maintenance => await new MaintenanceNewsInfo(newsType, parsedTime, title, url).Parse(token),
                        NewsType.Notice when url.Contains("/comic") => await new ComicNewsInfo(newsType, parsedTime, title, url).Parse(token),
                        _ => new NewsInfo(newsType, parsedTime, title, url),
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
