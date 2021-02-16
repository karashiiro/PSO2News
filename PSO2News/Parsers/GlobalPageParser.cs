using PSO2News.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;

namespace PSO2News.Parsers
{
    public class GlobalPageParser : NewsPageParser
    {
        private const string BaseUrl = "https://pso2.com/news";

        public override async IAsyncEnumerable<NewsInfo> GetNews(
            DateTime after,
            [EnumeratorCancellation] CancellationToken token)
        {
            var maxPages = -1;
            var curPage = 0;
            do
            {
                curPage++;

                var web = new HtmlWeb();
                var page = await web.LoadFromWebAsync(BaseUrl + "?page=" + curPage, token);

                if (maxPages == -1)
                {
                    var lastButtons = page.DocumentNode.SelectNodes("//li[@class='last']/a");
                    maxPages = lastButtons
                        .Select(n => n.GetAttributeValue("data-page", 0))
                        .Max();
                }

                var list = page.DocumentNode.SelectSingleNode("//ul[@class='news-section all-news all-section active']");

                foreach (var newsInfo in list.SelectNodes("li"))
                {
                    var typeNode = newsInfo.SelectSingleNode("div[@class='content']/div[@class='bottom']/p[@class='tag']");
                    var newsType = ParseUtil.GetNewsTypeNA(typeNode.InnerText);

                    var linkNode = newsInfo.SelectSingleNode("div[@class='content']/div[@class='bottom']/a[@class='read-more']");
                    var onClickFunc = linkNode.GetAttributeValue("onclick", "");
                    var url = GetNewsUrl(BaseUrl, onClickFunc, typeNode.InnerText.ToLowerInvariant().Replace(' ', '-'));

                    var titleNode = newsInfo.SelectSingleNode("div[@class='content']/h3");
                    var title = titleNode.InnerText.Trim();

                    var timeNode = newsInfo.SelectSingleNode("div[@class='content']/div[@class='bottom']/p[@class='date']");
                    var timeParts = ParseUtil.TimeRegexNA.Match(timeNode.InnerText).Groups;
                    var parsedTime = new DateTime(
                        int.Parse(timeParts["year"].Value),
                        int.Parse(timeParts["month"].Value),
                        int.Parse(timeParts["day"].Value),
                        0,
                        0,
                        0);
                    if (parsedTime <= after)
                    {
                        yield break;
                    }

                    yield return new NewsInfo(newsType, parsedTime, title, url);
                }
            } while (curPage < maxPages);
        }

        private static readonly Regex OnClickFuncRegex = new(@"\(ShowDetails\('(?<postId>\S+)', '\S+'\)\)", RegexOptions.Compiled);
        private static string GetNewsUrl(string baseUrl, string onClickFunc, string type)
        {
            var match = OnClickFuncRegex.Match(onClickFunc);
            if (match.Success)
            {
                return baseUrl + '/' + type + '/' + match.Groups["postId"];
            }

            return "";
        }
    }
}