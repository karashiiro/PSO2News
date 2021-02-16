using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace PSO2News.Content
{
    public class MaintenanceNewsInfo : NewsInfo
    {
        [JsonProperty("body")]
        public string Body { get; private set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; private set; }

        [JsonProperty("endTime")]
        public DateTime EndTime { get; private set; }

        public MaintenanceNewsInfo(NewsType type, DateTime timestamp, string title, string url) : base(type, timestamp, title, url) { }

        private static readonly Regex MaintenanceTimeRegex = new Regex(@"(?<year>\d{4})年(?<month>\d{1,2})月(?<day>\d{1,2})日（.）(?<startHour>\d{1,2}):(?<startMinute>\d{2}) ～ (?<endHour>\d{1,2}):(?<endMinute>\d{2})", RegexOptions.Compiled);

        internal async Task<MaintenanceNewsInfo> Initialize(CancellationToken token)
        {
            var web = new HtmlWeb();
            var page = await web.LoadFromWebAsync(Url, token);
            var content = page.DocumentNode.SelectSingleNode("//div[@class='newsWrap']");

            Body = content.InnerText;
            var timeParts = MaintenanceTimeRegex.Match(Body).Groups;

            StartTime = new DateTime(
                int.Parse(timeParts["year"].Value),
                int.Parse(timeParts["month"].Value),
                int.Parse(timeParts["day"].Value),
                int.Parse(timeParts["startHour"].Value),
                int.Parse(timeParts["startMinute"].Value),
                0);

            EndTime = new DateTime(
                int.Parse(timeParts["year"].Value),
                int.Parse(timeParts["month"].Value),
                int.Parse(timeParts["day"].Value),
                int.Parse(timeParts["endHour"].Value),
                int.Parse(timeParts["endMinute"].Value),
                0);

            return this;
        }
    }
}