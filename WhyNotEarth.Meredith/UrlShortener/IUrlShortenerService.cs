using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.UrlShortener
{
    public interface IUrlShortenerService
    {
        Task<ShortUrl> GetAsync(string key);

        Task<ShortUrl> AddAsync(string url);
    }
}