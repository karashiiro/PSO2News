namespace PSO2News.SiteInfo
{
    public class PSO2SiteInfo : NewsSiteInfo
    {
        public PSO2SiteInfo()
        {
            BaseUrl = "http://pso2.jp/players/news/";
            NextButtonSelector = "//li[@class='pager--next']/a";
            UlSelector = "//section[@class='topic--list']/ul";
            LinkSelector = "a";
            TypeSelector = "span[1]";
            TitleSelector = "span[2]";
            TimeSelector = "span[3]/time";
        }
    }
}