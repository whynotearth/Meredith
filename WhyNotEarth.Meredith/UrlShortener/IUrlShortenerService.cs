using System;
using System.Threading.Tasks;
using CSharpVitamins;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.UrlShortener
{
    public interface IUrlShortenerService
    {
        Task<ShortUrl> GetAsync(string id);

        Task<ShortUrl> AddAsync(string url);
    }

    internal class UrlShortenerService : IUrlShortenerService
    {
        private readonly IDbContext _dbContext;

        public UrlShortenerService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ShortUrl> GetAsync(string id)
        {
            var shortUrl = await _dbContext.ShortUrls.FirstOrDefaultAsync(item => item.Id == id);

            if (shortUrl is null)
            {
                throw new RecordNotFoundException($"URL {id} not found");
            }

            return shortUrl;
        }

        public async Task<ShortUrl> AddAsync(string url)
        {
            var id = ShortGuid.NewGuid();

            var uri = new Uri(url);

            var domain = uri.GetLeftPart(UriPartial.Authority);

            var uriBuilder = new UriBuilder(domain)
            {
                Path = $"t/{id}"
            };

            var shortUrl = new ShortUrl
            {
                Id = id,
                LongUrl = url,
                Url = uriBuilder.ToString()
            };

            _dbContext.ShortUrls.Add(shortUrl);

            await _dbContext.SaveChangesAsync();

            return shortUrl;
        }
    }
}