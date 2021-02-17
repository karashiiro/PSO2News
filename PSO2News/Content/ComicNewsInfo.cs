using System;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace PSO2News.Content
{
    public class ComicNewsInfo : NewsInfo
    {
        [JsonProperty("imageUrl")]
        public string ImageUrl { get; private set; }

        public ComicNewsInfo(NewsType type, DateTime timestamp, string title, string url) : base(type, timestamp, title, url) { }

        public async Task<ComicNewsInfo> Parse(CancellationToken token)
        {
            var web = new HtmlWeb();

            var page = await web.LoadFromWebAsync(Url, token);
            var imageNode = page.DocumentNode.SelectSingleNode("//p[@class='comic__image']/img");
            imageNode ??= page.DocumentNode.SelectSingleNode("//ul[@id='pso2es_comic']/li/img");
            ImageUrl = imageNode != null
                ? new Uri(new Uri(Url), imageNode.GetAttributeValue("src", "")).ToString()
                : "";

            return this;
        }
    }
}