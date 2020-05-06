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

        public async Task<byte[]> CreatePreviewAsync(int jumpStartId)
        {
            var jumpStart = await _dbContext.JumpStarts
                .Include(item => item.Articles)
                .ThenInclude(item => item.Image)
                .Include(item => item.Articles)
                .ThenInclude(item => item.Category)
                .ThenInclude(item => item.Image)
                .FirstOrDefaultAsync(item => item.Id == jumpStartId);

            return await _puppeteerService.BuildScreenshotAsync(jumpStart);
        }
    }
}