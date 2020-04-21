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
    }
}