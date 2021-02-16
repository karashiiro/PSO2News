namespace PSO2News.Parsers
{
    public class NGSPageParser : PSO2PageParser
    {
        public NGSPageParser()
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