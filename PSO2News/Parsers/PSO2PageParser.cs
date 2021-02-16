using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using HtmlAgilityPack;
using PSO2News.Content;

namespace PSO2News.Parsers
{
    public class PSO2PageParser : NewsPageParser
    {
        public override async IAsyncEnumerable<NewsInfo> GetNews(
            DateTime after,
            [EnumeratorCancellation] CancellationToken token)
        {
            var curUrl = "http://pso2.jp/players/news/";
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
                    var url = linkNode.GetAttributeValue("href", "");

                    var typeNode = linkNode.SelectSingleNode("span[1]");
                    var newsType = ParseUtil.GetNewsTypeJP(typeNode.InnerText);

                    var titleNode = linkNode.SelectSingleNode("span[2]");
                    var title = titleNode.InnerText;

                    var timeNode = linkNode.SelectSingleNode("span[3]/time");
                    var timeParts = ParseUtil.TimeRegexJP.Match(timeNode.InnerText).Groups;
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
    }
}