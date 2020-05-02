using System.Collections.Generic;
using System.IO;
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

        public async Task<Stream> CreatePreviewAsync(List<int> postIds)
        {
            var posts = await _dbContext.Posts
                .Where(item => postIds.Contains(item.Id))
                .Include(item => item.Image)
                .Include(item => item.Category)
                .ThenInclude(item => item.Image)
                .ToListAsync();

            return await _puppeteerService.BuildScreenshotAsync(posts);
        }
    }
}