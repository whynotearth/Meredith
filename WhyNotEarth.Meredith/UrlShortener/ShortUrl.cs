using System;

namespace WhyNotEarth.Meredith.UrlShortener
{
    public class ShortUrl
    {
        public int Id { get; set; }

        public string Key { get; set; } = null!;

        public string LongUrl { get; set; } = null!;

        public string Url { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}