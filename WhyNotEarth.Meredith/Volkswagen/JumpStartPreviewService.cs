using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartPreviewService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly PuppeteerService _puppeteerService;

        public JumpStartPreviewService(MeredithDbContext dbContext, PuppeteerService puppeteerService)
        {
            _dbContext = dbContext;
            _puppeteerService = puppeteerService;
        }

        public async Task<byte[]> CreatePreviewAsync(List<int> articleIds)
        {
            var articles = await _dbContext.Articles
                .Where(item => articleIds.Contains(item.Id))
                .Include(item => item.Image)
                .Include(item => item.Category)
                .ThenInclude(item => item.Image)
                .ToListAsync();

            return await _puppeteerService.BuildScreenshotAsync(articles);
        }
    }
}