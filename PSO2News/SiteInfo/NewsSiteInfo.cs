namespace PSO2News.SiteInfo
{
    public abstract class NewsSiteInfo
    {
        public string BaseUrl { get; protected set; }

        public string NextButtonSelector { get; protected set; }
        public string UlSelector { get; protected set; }

        public string LinkSelector { get; protected set; }
        public string TypeSelector { get; protected set; }
        public string TitleSelector { get; protected set; }
        public string TimeSelector { get; protected set; }
    }
}