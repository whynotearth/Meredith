using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartPreviewService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly JumpStartPlanService _jumpStartPlanService;
        private readonly PuppeteerService _puppeteerService;

        public JumpStartPreviewService(MeredithDbContext dbContext, PuppeteerService puppeteerService,
            JumpStartPlanService jumpStartPlanService)
        {
            _dbContext = dbContext;
            _puppeteerService = puppeteerService;
            _jumpStartPlanService = jumpStartPlanService;
        }

        public async Task<byte[]> CreatePreviewAsync(DateTime date, List<int> articleIds)
        {
            if (articleIds.Count > _jumpStartPlanService.MaximumArticlesPerDayCount)
            {
                throw new InvalidActionException(
                    $"Maximum {_jumpStartPlanService.MaximumArticlesPerDayCount} articles are allowed per email");
            }

            var articles = await _dbContext.Articles
                .Include(item => item.Image)
                .Include(item => item.Category)
                .ThenInclude(item => item.Image)
                .Where(item => articleIds.Contains(item.Id)).ToListAsync();

            // Keep the order of the articles
            articles = articles.OrderBy(item => articleIds.IndexOf(item.Id)).ToList();

            return await _puppeteerService.BuildScreenshotAsync(date, articles);
        }
    }
}