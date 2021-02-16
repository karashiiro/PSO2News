namespace PSO2News.SiteInfo
{
    public class NGSSiteInfo : NewsSiteInfo
    {
        public NGSSiteInfo()
        {
            BaseUrl = "https://new-gen.pso2.jp/cbt/players/news/";
            NextButtonSelector = "//li[@class='pager__next']/a";
            UlSelector = "//ul[@class='newslist']";
            LinkSelector = "a";
            TypeSelector = "span[1]";
            TitleSelector = "span[2]";
            TimeSelector = "time";
        }
    }
}