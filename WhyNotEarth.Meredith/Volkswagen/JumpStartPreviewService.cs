using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Exceptions;

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

        public async Task<byte[]> CreatePreviewAsync(int jumpStartId, List<int>? articleIds)
        {
            var jumpStart = await _dbContext.JumpStarts
                .Include(item => item.Articles)
                .ThenInclude(item => item.Image)
                .Include(item => item.Articles)
                .ThenInclude(item => item.Category)
                .ThenInclude(item => item.Image)
                .FirstOrDefaultAsync(item => item.Id == jumpStartId);

            if (jumpStart is null)
            {
                throw new RecordNotFoundException($"JumpStart {jumpStartId} not found");
            }

            List<Article> articles;
            if (articleIds is null)
            {
                articles = jumpStart.Articles;
            }
            else
            {
                articles = jumpStart.Articles.Where(item => articleIds.Contains(item.Id)).ToList();
            }

            return await _puppeteerService.BuildScreenshotAsync(jumpStart, articles);
        }
    }
}