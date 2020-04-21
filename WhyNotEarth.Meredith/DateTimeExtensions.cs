using System;
using NodaTime;

namespace WhyNotEarth.Meredith
{
    public static class DateTimeExtensions
    {
        public static string InZone(this DateTime datetime, string timeZone, string format)
        {
            var dateTimeZone = DateTimeZoneProviders.Tzdb[timeZone];

            var instant = Instant.FromDateTimeUtc(datetime);
            var zonedDateTime = instant.InZone(dateTimeZone);

            return zonedDateTime.ToString(format, System.Globalization.CultureInfo.CurrentCulture);
        }
    }
}