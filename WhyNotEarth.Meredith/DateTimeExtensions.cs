using System;

namespace WhyNotEarth.Meredith
{
    public static class DateTimeExtensions
    {
        public static string InZone(this DateTime datetime, int timeZoneOffset, string format)
        {
            var dateTimeOffset = new DateTimeOffset(datetime);
            var localDateTime = dateTimeOffset.ToOffset(new TimeSpan(0, -timeZoneOffset, 0));

            return localDateTime.ToString(format);
        }

        public static string InZone(this DateTime datetime, string timeZoneId, string format)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(datetime, timeZone);

            return localDateTime.ToString(format);
        }

        public static DateTime InZone(this DateTime datetime, string timeZoneId)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(datetime, timeZoneId);
        }
    }
}