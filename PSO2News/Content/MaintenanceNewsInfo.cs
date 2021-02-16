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

        [JsonProperty("endTimeUndecided")]
        public bool EndTimeUndecided { get; private set; }

        [JsonProperty("unreadable")]
        public bool Unreadable { get; private set; }

        [JsonProperty("reason")]
        public MaintenanceNewsReason Reason { get; private set; }

        [JsonProperty("affectedService")]
        public MaintenanceAffectedService AffectedService { get; private set; }

        public MaintenanceNewsInfo(NewsType type, DateTime timestamp, string title, string url) : base(type, timestamp, title, url) { }

        private static readonly Regex TimeRegex = new(@"(?<year>\d{4})年(?<month>\d{1,2})月(?<day>\d{1,2})日（.）\s*(?<startHour>\d{1,2}):(?<startMinute>\d{2})\s?～\s?(?<endHour>\d{1,2}):(?<endMinute>\d{2})", RegexOptions.Compiled);
        private static readonly Regex TimeRegexEx = new(@"本日(?<startHour>\d{1,2}):(?<startMinute>\d{2}).+メンテナンス終了予定時(刻|間)\s(?<month>\d{1,2})月(?<day>\d{1,2})日（.）\s*(?<endHour>\d{1,2}):(?<endMinute>\d{2})", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex TimeCloudRegex = new(@"下記の予定にて、“クラウド”版のゲームサーバーの\S*メンテナンスを行います。.+(?<month>\d{1,2})月(?<day>\d{1,2})日（.）\s*(?<startHour>\d{1,2}):(?<startMinute>\d{2})\s?～\s?(?<endHour>\d{1,2}):(?<endMinute>\d{2})", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex TimeNoYearRegex = new(@"(?<month>\d{1,2})月(?<day>\d{1,2})日（.）\s*(?<startHour>\d{1,2}):(?<startMinute>\d{2})\s?～\s?(?<endHour>\d{1,2}):(?<endMinute>\d{2})", RegexOptions.Compiled);
        private static readonly Regex TimeNoYearMultiDayRegex = new(@"(?<startMonth>\d{1,2})月(?<startDay>\d{1,2})日（.）\s*(?<startHour>\d{1,2}):(?<startMinute>\d{2})\s?～\s?(?<endMonth>\d{1,2})月(?<endDay>\d{1,2})日（.）(?<endHour>\d{1,2}):(?<endMinute>\d{2})", RegexOptions.Compiled);
        private static readonly Regex TimeUndecidedRegex = new(@"本日(?<startHour>\d{1,2}):(?<startMinute>\d{2})より.+\S*メンテナンス終了予定時刻\s+(?<year>\d{4})年(?<month>\d{1,2})月(?<day>\d{1,2})日（.）\s+未定", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex TimeRecapRegex = new(@"本日(?<startHour>\d{1,2}):(?<startMinute>\d{2})からの\S*メンテナンスは、(?<endHour>\d{1,2}):(?<endMinute>\d{2})を?もちまして終了(いた)?しました。", RegexOptions.Compiled);
        private static readonly Regex TimeRecapMultiDayRegex = new(@"(?<startMonth>\d{1,2})月(?<startDay>\d{1,2})日（.）\s*(?<startHour>\d{1,2}):(?<startMinute>\d{2})からの?\S*メンテナンスは、(?<endMonth>\d{1,2})月(?<endDay>\d{1,2})日（.）(?<endHour>\d{1,2}):(?<endMinute>\d{2})をもちまして終了(いた)?しました。", RegexOptions.Compiled);
        private static readonly Regex TimeRecapCloudRegex = new(@"(?<year>\d{4})\/(?<month>\d{1,2})\/(?<day>\d{1,2}).+本日(?<startHour>\d{1,2}):(?<startMinute>\d{2})からの“クラウド”版の?\S*メンテナンスは、(?<endHour>\d{1,2}):(?<endMinute>\d{2})をもちまして終了(いた)?しました。", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex TimeRecapCloudRegexEx = new(@"本日(?<startHour>\d{1,2}):(?<startMinute>\d{2})からの“クラウド”版定期メンテナンスは、(?<endHour>\d{1,2}):(?<endMinute>\d{2})をもちまして終了(いた)?しました。", RegexOptions.Compiled);
        private static readonly Regex TimeRecapRegexEx = new(@"本日(?<startHour>\d{1,2}):(?<startMinute>\d{2}).+(?<endHour>\d{1,2}):(?<endMinute>\d{2})", RegexOptions.Compiled);
        private static readonly Regex TimeRecapRegexEx2 = new(@"本日.*(?<startHour>\d{1,2}):(?<startMinute>\d{2})より行.*メンテナンスは、(?<endHour>\d{1,2}):(?<endMinute>\d{2})", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex TimeAdjustRegex = new(@"→　(?<month>\d{1,2})月(?<day>\d{1,2})日（.）\s*(?<startHour>\d{1,2}):(?<startMinute>\d{2})\s?～\s?(?<endHour>\d{1,2}):(?<endMinute>\d{2})", RegexOptions.Compiled);
        private static readonly Regex TimeAdjustRegexEx = new(@"本日(?<startHour>\d{1,2}):(?<startMinute>\d{2})より行っております定期メンテナンスにつきまして、.+\S*メンテナンス終了予定時刻.+(?<year>\d{4})年(?<month>\d{1,2})月(?<day>\d{1,2})日（.）(?<endHour>\d{1,2}):(?<endMinute>\d{2})", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex TimeAdjustUndecidedRegex = new(@"本日(?<startHour>\d{1,2}):(?<startMinute>\d{2}).*(?<year>\d{4})年(?<month>\d{1,2})月(?<day>\d{1,2})日（.）未定", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex TimeAdjustUndecidedNoYearRegex = new(@"本日(?<startHour>\d{1,2}):(?<startMinute>\d{2}).*(?<month>\d{1,2})月(?<day>\d{1,2})日（.）未定", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex TimeAdjustUndecidedCloudRegex = new(@"(?<year>\d{4})\/(?<month>\d{1,2})\/(?<day>\d{1,2}) (?<startHour>\d{1,2}):(?<startMinute>\d{2}).+“クラウド”版の?\S*メンテナンス.+未定", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex TimeEmergencyRegex = new(@"緊急メンテナンス予定時間\s+(?<startMonth>\d{1,2})月(?<startDay>\d{1,2})日（.）\s*(?<startHour>\d{1,2}):(?<startMinute>\d{2})\s?～\s?(?<endMonth>\d{1,2})月(?<endDay>\d{1,2})日（.）(?<endHour>\d{1,2}):(?<endMinute>\d{2})", RegexOptions.Compiled);
        private static readonly Regex TimeEmergencyUndecidedRegex = new(@"緊急メンテナンス予定時間\s+(?<month>\d{1,2})月(?<day>\d{1,2})日（.）\s*(?<startHour>\d{1,2}):(?<startMinute>\d{2})\s?～\s?(終了時刻)?未定", RegexOptions.Compiled);
        private static readonly Regex TimeEmergencyUndecidedRegexEx = new(@"(?<month>\d{1,2})\/(?<day>\d{1,2}).+(?<startHour>\d{1,2})：(?<startMinute>\d{2}).*緊急メンテナンス", RegexOptions.Compiled);
        private static readonly Regex TimeServerEquipmentRegex = new(@"(?<month>\d{1,2})月(?<day>\d{1,2})日（.）の?(?<startHour>\d{1,2}):(?<startMinute>\d{2})～(?<endHour>\d{1,2}):(?<endMinute>\d{2})までの間、サーバー機器メンテナンスを実施(いた)?します。", RegexOptions.Compiled);
        private static readonly Regex TimeNARegex = new(@"Maintenance Starts:\s*(?<startMonth>\d{1,2})\/(?<startDay>\d{1,2})\s*(?<startHour>\d{1,2}):(?<startMinute>\d{2})\s*(?<startMeridiem>\S*).*(?<endMonth>\d{1,2})\/(?<endDay>\d{1,2})\s*(?<endHour>\d{1,2}):(?<endMinute>\d{2})\s*(?<endMeridiem>\S*)", RegexOptions.Compiled | RegexOptions.Singleline);

        public async Task<MaintenanceNewsInfo> Parse(CancellationToken token)
        {
            var web = new HtmlWeb();

            var page = await web.LoadFromWebAsync(Url, token);
            var content = page.DocumentNode.SelectSingleNode("//div[@class='newsWrap']");
            content ??= page.DocumentNode.SelectSingleNode("//div[@id='news-block']"); // Old-style wrappers

            Body = content.InnerText;
            Reason = MaintenanceNewsReason.Announce;
            AffectedService = MaintenanceAffectedService.All;

            var time = TimeRegex.Match(Body);
            var timeEx = TimeRegexEx.Match(Body);
            var timeCloud = TimeCloudRegex.Match(Body);
            var timeNoYear = TimeNoYearRegex.Match(Body);
            var timeNoYearMultiDay = TimeNoYearMultiDayRegex.Match(Body);
            var timeUndecided = TimeUndecidedRegex.Match(Body);
            var timeRecap = TimeRecapRegex.Match(Body);
            var timeRecapMultiDay = TimeRecapMultiDayRegex.Match(Body);
            var timeRecapCloud = TimeRecapCloudRegex.Match(Body);
            var timeRecapCloudEx = TimeRecapCloudRegexEx.Match(Body);
            var timeRecapEx = TimeRecapRegexEx.Match(Body);
            var timeRecapEx2 = TimeRecapRegexEx2.Match(Body);
            var timeAdjust = TimeAdjustRegex.Match(Body);
            var timeAdjustEx = TimeAdjustRegexEx.Match(Body);
            var timeAdjustUndecided = TimeAdjustUndecidedRegex.Match(Body);
            var timeAdjustUndecidedNoYear = TimeAdjustUndecidedNoYearRegex.Match(Body);
            var timeAdjustUndecidedCloud = TimeAdjustUndecidedCloudRegex.Match(Body);
            var timeEmergency = TimeEmergencyRegex.Match(Body);
            var timeEmergencyUndecided = TimeEmergencyUndecidedRegex.Match(Body);
            var timeEmergencyUndecidedEx = TimeEmergencyUndecidedRegexEx.Match(Body);
            var timeServerEquipment = TimeServerEquipmentRegex.Match(Body);
            var timeNA = TimeNARegex.Match(Body);

            int year, month, day, startHour, startMinute, endHour, endMinute;
            if (time.Success)
            {
                var timeParts = time.Groups;

                year = int.Parse(timeParts["year"].Value);
                month = int.Parse(timeParts["month"].Value);
                day = int.Parse(timeParts["day"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);
                endHour = int.Parse(timeParts["endHour"].Value);
                endMinute = int.Parse(timeParts["endMinute"].Value);
            }
            else if (timeEx.Success)
            {
                var timeParts = timeEx.Groups;

                year = Timestamp.Year;
                month = int.Parse(timeParts["month"].Value);
                day = int.Parse(timeParts["day"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);
                endHour = int.Parse(timeParts["endHour"].Value);
                endMinute = int.Parse(timeParts["endMinute"].Value);
            }
            else if (timeCloud.Success)
            {
                var timeParts = timeCloud.Groups;
                AffectedService = MaintenanceAffectedService.Cloud;

                year = Timestamp.Year;
                month = int.Parse(timeParts["month"].Value);
                day = int.Parse(timeParts["day"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);
                endHour = int.Parse(timeParts["endHour"].Value);
                endMinute = int.Parse(timeParts["endMinute"].Value);
            }
            else if (timeNoYear.Success)
            {
                var timeParts = timeNoYear.Groups;

                year = Timestamp.Year;
                month = int.Parse(timeParts["month"].Value);
                day = int.Parse(timeParts["day"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);
                endHour = int.Parse(timeParts["endHour"].Value);
                endMinute = int.Parse(timeParts["endMinute"].Value);
            }
            else if (timeNoYearMultiDay.Success)
            {
                var timeParts = timeNoYearMultiDay.Groups;

                year = Timestamp.Year;
                var startMonth = int.Parse(timeParts["startMonth"].Value);
                var startDay = int.Parse(timeParts["startDay"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);
                var endMonth = int.Parse(timeParts["endMonth"].Value);
                var endDay = int.Parse(timeParts["endDay"].Value);
                endHour = int.Parse(timeParts["endHour"].Value);
                endMinute = int.Parse(timeParts["endMinute"].Value);

                StartTime = new DateTime(year, startMonth, startDay, startHour, startMinute, 0);
                EndTime = new DateTime(year, endMonth, endDay, endHour, endMinute, 0);

                return this;
            }
            else if (timeUndecided.Success)
            {
                var timeParts = timeUndecided.Groups;

                year = int.Parse(timeParts["year"].Value);
                month = int.Parse(timeParts["month"].Value);
                day = int.Parse(timeParts["day"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);

                StartTime = new DateTime(year, month, day, startHour, startMinute, 0);
                EndTime = default;
                EndTimeUndecided = true;

                return this;
            }
            else if (timeRecap.Success)
            {
                var timeParts = timeRecap.Groups;
                Reason = MaintenanceNewsReason.Recap;

                year = Timestamp.Year;
                month = Timestamp.Month;
                day = Timestamp.Day;
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);
                endHour = int.Parse(timeParts["endHour"].Value);
                endMinute = int.Parse(timeParts["endMinute"].Value);
            }
            else if (timeRecapMultiDay.Success)
            {
                var timeParts = timeRecapMultiDay.Groups;
                Reason = MaintenanceNewsReason.Recap;

                year = Timestamp.Year;
                var startMonth = int.Parse(timeParts["startMonth"].Value);
                var startDay = int.Parse(timeParts["startDay"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);
                var endMonth = int.Parse(timeParts["endMonth"].Value);
                var endDay = int.Parse(timeParts["endDay"].Value);
                endHour = int.Parse(timeParts["endHour"].Value);
                endMinute = int.Parse(timeParts["endMinute"].Value);

                StartTime = new DateTime(year, startMonth, startDay, startHour, startMinute, 0);
                EndTime = new DateTime(year, endMonth, endDay, endHour, endMinute, 0);

                return this;
            }
            else if (timeRecapCloud.Success)
            {
                var timeParts = timeRecapCloud.Groups;
                Reason = MaintenanceNewsReason.Recap;
                AffectedService = MaintenanceAffectedService.Cloud;

                year = int.Parse(timeParts["year"].Value);
                month = int.Parse(timeParts["month"].Value);
                day = int.Parse(timeParts["day"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);
                endHour = int.Parse(timeParts["endHour"].Value);
                endMinute = int.Parse(timeParts["endMinute"].Value);
            }
            else if (timeRecapCloudEx.Success)
            {
                var timeParts = timeRecapCloudEx.Groups;
                Reason = MaintenanceNewsReason.Recap;
                AffectedService = MaintenanceAffectedService.Cloud;

                year = Timestamp.Year;
                month = Timestamp.Month;
                day = Timestamp.Day;
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);
                endHour = int.Parse(timeParts["endHour"].Value);
                endMinute = int.Parse(timeParts["endMinute"].Value);
            }
            else if (timeRecapEx.Success)
            {
                var timeParts = timeRecapEx.Groups;
                Reason = MaintenanceNewsReason.Recap;

                year = Timestamp.Year;
                month = Timestamp.Month;
                day = Timestamp.Day;
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);
                endHour = int.Parse(timeParts["endHour"].Value);
                endMinute = int.Parse(timeParts["endMinute"].Value);
            }
            else if (timeRecapEx2.Success)
            {
                var timeParts = timeRecapEx2.Groups;
                Reason = MaintenanceNewsReason.Recap;

                year = Timestamp.Year;
                month = Timestamp.Month;
                day = Timestamp.Day;
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);
                endHour = int.Parse(timeParts["endHour"].Value);
                endMinute = int.Parse(timeParts["endMinute"].Value);
            }
            else if (timeAdjust.Success)
            {
                var timeParts = timeAdjust.Groups;
                Reason = MaintenanceNewsReason.Adjust;

                year = Timestamp.Year;
                month = int.Parse(timeParts["month"].Value);
                day = int.Parse(timeParts["day"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);
                endHour = int.Parse(timeParts["endHour"].Value);
                endMinute = int.Parse(timeParts["endMinute"].Value);
            }
            else if (timeAdjustEx.Success)
            {
                var timeParts = timeAdjustEx.Groups;
                Reason = MaintenanceNewsReason.Adjust;

                year = int.Parse(timeParts["year"].Value);
                month = int.Parse(timeParts["month"].Value);
                day = int.Parse(timeParts["day"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);
                endHour = int.Parse(timeParts["endHour"].Value);
                endMinute = int.Parse(timeParts["endMinute"].Value);
            }
            else if (timeAdjustUndecided.Success)
            {
                var timeParts = timeAdjustUndecided.Groups;
                Reason = MaintenanceNewsReason.Adjust;

                year = int.Parse(timeParts["year"].Value);
                month = int.Parse(timeParts["month"].Value);
                day = int.Parse(timeParts["day"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);

                StartTime = new DateTime(year, month, day, startHour, startMinute, 0);
                EndTime = default;
                EndTimeUndecided = true;

                return this;
            }
            else if (timeAdjustUndecidedNoYear.Success)
            {
                var timeParts = timeAdjustUndecidedNoYear.Groups;
                Reason = MaintenanceNewsReason.Adjust;

                year = Timestamp.Year;
                month = int.Parse(timeParts["month"].Value);
                day = int.Parse(timeParts["day"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);

                StartTime = new DateTime(year, month, day, startHour, startMinute, 0);
                EndTime = default;
                EndTimeUndecided = true;

                return this;
            }
            else if (timeAdjustUndecidedCloud.Success)
            {
                var timeParts = timeAdjustUndecidedCloud.Groups;
                Reason = MaintenanceNewsReason.Adjust;
                AffectedService = MaintenanceAffectedService.Cloud;

                year = int.Parse(timeParts["year"].Value);
                month = int.Parse(timeParts["month"].Value);
                day = int.Parse(timeParts["day"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);

                StartTime = new DateTime(year, month, day, startHour, startMinute, 0);
                EndTime = default;
                EndTimeUndecided = true;

                return this;
            }
            else if (timeEmergency.Success)
            {
                var timeParts = timeEmergency.Groups;
                Reason = MaintenanceNewsReason.Emergency;

                year = Timestamp.Year;
                var startMonth = int.Parse(timeParts["startMonth"].Value);
                var startDay = int.Parse(timeParts["startDay"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);
                var endMonth = int.Parse(timeParts["endMonth"].Value);
                var endDay = int.Parse(timeParts["endDay"].Value);
                endHour = int.Parse(timeParts["endHour"].Value);
                endMinute = int.Parse(timeParts["endMinute"].Value);

                StartTime = new DateTime(year, startMonth, startDay, startHour, startMinute, 0);
                EndTime = new DateTime(year, endMonth, endDay, endHour, endMinute, 0);

                return this;
            }
            else if (timeEmergencyUndecided.Success)
            {
                var timeParts = timeEmergencyUndecided.Groups;
                Reason = MaintenanceNewsReason.Emergency;

                year = Timestamp.Year;
                month = int.Parse(timeParts["month"].Value);
                day = int.Parse(timeParts["day"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);

                StartTime = new DateTime(year, month, day, startHour, startMinute, 0);
                EndTime = default;
                EndTimeUndecided = true;

                return this;
            }
            else if (timeEmergencyUndecidedEx.Success)
            {
                var timeParts = timeEmergencyUndecidedEx.Groups;
                Reason = MaintenanceNewsReason.Emergency;

                year = Timestamp.Year;
                month = int.Parse(timeParts["month"].Value);
                day = int.Parse(timeParts["day"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);

                StartTime = new DateTime(year, month, day, startHour, startMinute, 0);
                EndTime = default;
                EndTimeUndecided = true;

                return this;
            }
            else if (timeServerEquipment.Success)
            {
                var timeParts = timeServerEquipment.Groups;
                AffectedService = MaintenanceAffectedService.ServerEquipment;

                year = Timestamp.Year;
                month = int.Parse(timeParts["month"].Value);
                day = int.Parse(timeParts["day"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);
                endHour = int.Parse(timeParts["endHour"].Value);
                endMinute = int.Parse(timeParts["endMinute"].Value);
            }
            else if (timeNA.Success)
            {
                if (Title.ToLowerInvariant().Contains("emergency") && !Title.ToLowerInvariant().Contains("now open"))
                {
                    Reason = MaintenanceNewsReason.Emergency;
                }

                var timeParts = timeNA.Groups;

                year = Timestamp.Year;

                var startMonth = int.Parse(timeParts["startMonth"].Value);
                var startDay = int.Parse(timeParts["startDay"].Value);
                startHour = int.Parse(timeParts["startHour"].Value);
                startMinute = int.Parse(timeParts["startMinute"].Value);
                var startMeridiem = timeParts["startMeridiem"].Value;
                if (startMeridiem.ToLowerInvariant() == "pm")
                {
                    startHour += 12;
                }

                var endMonth = int.Parse(timeParts["endMonth"].Value);
                var endDay = int.Parse(timeParts["endDay"].Value);
                endHour = int.Parse(timeParts["endHour"].Value);
                endMinute = int.Parse(timeParts["endMinute"].Value);
                var endMeridiem = timeParts["endMeridiem"].Value;
                if (endMeridiem.ToLowerInvariant() == "pm")
                {
                    endHour += 12;
                }

                StartTime = new DateTime(year, startMonth, startDay, startHour, startMinute, 0);
                EndTime = new DateTime(year, endMonth, endDay, endHour, endMinute, 0);

                return this;
            }
            else
            {
                Unreadable = true;
                return this;
            }

            StartTime = new DateTime(year, month, day, startHour, startMinute, 0);
            EndTime = new DateTime(year, month, day, endHour, endMinute, 0);

            return this;
        }

        public enum MaintenanceNewsReason
        {
            Announce,
            Recap,
            Adjust,
            Emergency,
        }

        public enum MaintenanceAffectedService
        {
            All,
            Cloud,
            ServerEquipment,
        }
    }
}