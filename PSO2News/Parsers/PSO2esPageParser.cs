using System;
using HtmlAgilityPack;

namespace PSO2News.Parsers
{
    public class PSO2esPageParser : PSO2PageParser
    {
        public PSO2esPageParser()
        {
            BaseUrl = "https://pso2.jp/es/players/news/";
        }

        protected override string GetLinkUrl(HtmlNode linkNode)
        {
            var fragment = base.GetLinkUrl(linkNode);
            return BaseUrl + fragment;
        }

        protected override string GetNextPageUrl(HtmlNode nextButton)
        {
            var fragment = nextButton?.GetAttributeValue("href", null);
            if (fragment == null)
            {
                return null;
            }
            
            return (BaseUrl + fragment).Replace("&amp;", "&");
        }
    }
}