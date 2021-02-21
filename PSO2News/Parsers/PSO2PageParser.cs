using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PSO2News.Content;

namespace PSO2News.Parsers
{
    public class PSO2PageParser : NewsPageParser
    {
        protected string BaseUrl { get; set; }

        protected string NextButtonSelector { get; set; }
        protected string UlSelector { get; set; }

        protected string LinkSelector { get; set; }
        protected string TypeSelector { get; set; }
        protected string TitleSelector { get; set; }
        protected string TimeSelector { get; set; }

        public PSO2PageParser()
        {
            BaseUrl = "http://pso2.jp/players/news/";
            NextButtonSelector = "//li[@class='pager--next']/a";
            UlSelector = "//section[@class='topic--list']/ul";
            LinkSelector = "a";
            TypeSelector = "span[1]";
            TitleSelector = "span[2]";
            TimeSelector = "span[3]/time";
        }

        public override async IAsyncEnumerable<NewsInfo> GetNews(
            DateTime after,
            [EnumeratorCancellation] CancellationToken token)
        {
            var curUrl = BaseUrl;
            do
            {
                var web = new HtmlWeb();
                var page = await web.LoadFromWebAsync(curUrl, token);

                var nextButton = page.DocumentNode.SelectSingleNode(NextButtonSelector);
                curUrl = GetNextPageUrl(nextButton);

                var list = page.DocumentNode.SelectSingleNode(UlSelector);

                var jobs = list.SelectNodes("li")
                    .Select(newsInfo => Task.Run(async () =>
                    {
                        var linkNode = newsInfo.SelectSingleNode(LinkSelector);
                        var url = GetLinkUrl(linkNode);

                        var typeNode = linkNode.SelectSingleNode(TypeSelector);
                        var newsType = ParseUtil.GetNewsTypeJP(typeNode.InnerText);

                        var titleNode = linkNode.SelectSingleNode(TitleSelector);
                        var title = titleNode.InnerText.Trim();

                        var timeNode = linkNode.SelectSingleNode(TimeSelector);
                        var timeParts = ParseUtil.TimeRegexJP.Match(timeNode.InnerText).Groups;
                        var parsedTime = new DateTime(int.Parse(timeParts["year"].Value), int.Parse(timeParts["month"].Value), int.Parse(timeParts["day"].Value), int.Parse(timeParts["hour"].Success ? timeParts["hour"].Value : "0"), int.Parse(timeParts["minute"].Success ? timeParts["minute"].Value : "0"), 0);
                        parsedTime = parsedTime.AddHours(-9);
                        if (parsedTime <= after)
                        {
                            return null;
                        }

                        return newsType switch
                        {
                            NewsType.Maintenance => await new MaintenanceNewsInfo(newsType, parsedTime, title, url).Parse(token),
                            NewsType.Announcement when url.Contains("/comic") => await new ComicNewsInfo(newsType, parsedTime, title, url).Parse(token),
                            _ => new NewsInfo(newsType, parsedTime, title, url),
                        };
                    }, token))
                    .ToList();

                await Task.WhenAll(jobs);

                var newsInfos = jobs
                    .Select(job => job.Result)
                    .ToList();

                foreach (var newsInfo in newsInfos.Where(newsInfo => newsInfo != null))
                {
                    yield return newsInfo;
                }

                if (newsInfos.Any(newsInfo => newsInfo == null))
                {
                    yield break;
                }
            } while (curUrl != null);
        }

        protected virtual string GetLinkUrl(HtmlNode linkNode)
        {
            return linkNode.GetAttributeValue("href", "");
        }

        protected virtual string GetNextPageUrl(HtmlNode nextButton)
        {
            return nextButton?.GetAttributeValue("href", null);
        }
    }
}