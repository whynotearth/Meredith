using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartService
    {
        private readonly ArticleService _articleService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly MeredithDbContext _dbContext;

        public JumpStartService(MeredithDbContext dbContext, ArticleService articleService,
            IBackgroundJobClient backgroundJobClient)
        {
            _dbContext = dbContext;
            _articleService = articleService;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<List<JumpStart>> ListAsync()
        {
            var jumpStarts = await _dbContext.JumpStarts
                .Include(item => item.Articles)
                .ThenInclude(item => item.Category)
                .ThenInclude(item => item.Image)
                .Where(item => item.Status == JumpStartStatus.Preview)
                .ToListAsync();

            foreach (var jumpStart in jumpStarts)
            {
                if (!jumpStart.Articles.Any())
                {
                    // If the JumpStart doesn't have any articles assigned 
                    // Use the default ones (Maximum number of possible articles order by priority)
                    jumpStart.Articles = await _dbContext.Articles
                        .Include(item => item.Category)
                        .ThenInclude(item => item.Image)
                        .Where(item => item.JumpStartId == null && item.Date == jumpStart.DateTime.Date)
                        .OrderBy(item => item.Category.Priority)
                        .Take(_articleService.MaximumArticlesPerDayCount)
                        .ToListAsync();
                }
            }

            return jumpStarts;
        }

        public async Task Edit(int jumpStartId, DateTime dateTime, List<string> distributionGroups,
            List<int> articleIds)
        {
            if (articleIds.Count > _articleService.MaximumArticlesPerDayCount)
            {
                throw new InvalidActionException(
                    $"Maximum {_articleService.MaximumArticlesPerDayCount} articles are allowed per email");
            }

            var jumpStart = await GetAsync(jumpStartId);

            var newArticles = await _dbContext.Articles.Where(item => articleIds.Contains(item.Id))
                .ToListAsync();

            jumpStart.DateTime = dateTime;
            jumpStart.DistributionGroups = string.Join(',', distributionGroups);
            jumpStart.Articles = newArticles;
            _dbContext.JumpStarts.Update(jumpStart);

            foreach (var article in newArticles)
            {
                article.JumpStart = jumpStart;
                article.Order = articleIds.IndexOf(article.Id);
            }

            _dbContext.UpdateRange(newArticles);

            await _dbContext.SaveChangesAsync();
        }

        public async Task SendAsync()
        {
            var jumpStarts = await _dbContext.JumpStarts.Where(item =>
                    item.Status == JumpStartStatus.Preview && item.DateTime < DateTime.UtcNow.AddMinutes(15))
                .ToListAsync();

            foreach (var jumpStart in jumpStarts)
            {
                jumpStart.Status = JumpStartStatus.ReadyToSend;
            }

            _dbContext.UpdateRange(jumpStarts);
            await _dbContext.SaveChangesAsync();

            foreach (var jumpStart in jumpStarts)
            {
                _backgroundJobClient.Enqueue<JumpStartPdfService>(service =>
                    service.CreatePdfAsync(jumpStart.Id));
            }
        }

        private async Task<JumpStart> GetAsync(int jumpStartId)
        {
            var jumpStart = await _dbContext.JumpStarts.FirstOrDefaultAsync(item => item.Id == jumpStartId);

            if (jumpStart is null)
            {
                throw new RecordNotFoundException($"JumpStart {jumpStartId} not found");
            }

            if (jumpStart.Status != JumpStartStatus.Preview)
            {
                throw new InvalidActionException($"JumpStart {jumpStartId} had already sent");
            }

            return jumpStart;
        }
    }
}