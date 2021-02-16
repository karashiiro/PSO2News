using System.Text.RegularExpressions;

namespace PSO2News.Parsers
{
    public static class ParseUtil
    {
        public static readonly Regex TimeRegexJP = new Regex(@"(?<year>\d{4})\/(?<month>\d{2})\/(?<day>\d{2}) (?<hour>\d{2}):(?<minute>\d{2})", RegexOptions.Compiled);

        public static NewsType GetNewsTypeJP(string typeName)
        {
            return typeName switch
            {
                "お知らせ" => NewsType.Notice,
                "復旧" => NewsType.Recovery,
                "重要" => NewsType.Important,
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
    }
}