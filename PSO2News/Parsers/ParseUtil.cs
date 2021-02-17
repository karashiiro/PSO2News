using System.Text.RegularExpressions;

namespace PSO2News.Parsers
{
    public static class ParseUtil
    {
        public static readonly Regex TimeRegexJP = new(@"(?<year>\d{4})\/(?<month>\d{2})\/(?<day>\d{2})( (?<hour>\d{2}):(?<minute>\d{2}))?", RegexOptions.Compiled);
        public static readonly Regex TimeRegexNA = new(@"(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})", RegexOptions.Compiled);

        public static NewsType GetNewsTypeJP(string typeName)
        {
            return typeName switch
            {
                "お知らせ" => NewsType.Announcement,
                "復旧" => NewsType.Recovery,
                "重要" => NewsType.Important,
                "配信" => NewsType.Delivery,
                "続報" => NewsType.FollowUp,
                "FAQ" => NewsType.FAQ,
                "WEB" => NewsType.Web,
                "対応状況" => NewsType.Bugs,
                "メンテナンス" => NewsType.Maintenance,
                "アップデート" => NewsType.Update,
                "イベント" => NewsType.Event,
                "キャンペーン" => NewsType.Campaign,
                "メディア" => NewsType.Media,
                _ => NewsType.Unknown,
            };
        }

        public static NewsType GetNewsTypeNA(string typeName)
        {
            return typeName switch
            {
                "Announcements" => NewsType.Announcement,
                "Scratch Tickets" => NewsType.ScratchTicket,
                "Server Info" => NewsType.Maintenance,
                "Urgent Quests" => NewsType.UrgentQuest,
                "Blog" => NewsType.Blog,
                _ => NewsType.Unknown,
            };
        }
    }
}