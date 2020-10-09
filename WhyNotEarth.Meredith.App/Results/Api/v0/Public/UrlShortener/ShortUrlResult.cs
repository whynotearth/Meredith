using WhyNotEarth.Meredith.UrlShortener;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.UrlShortener
{
    public class ShortUrlResult
    {
        public string Url { get; }

        public ShortUrlResult(ShortUrl shortUrl)
        {
            Url = shortUrl.LongUrl;
        }
    }
}