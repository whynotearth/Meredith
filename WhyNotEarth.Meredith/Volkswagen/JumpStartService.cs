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
        private const int MaximumArticlesPerDayCount = 5;

        private readonly ArticleService _articleService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly MeredithDbContext _dbContext;

        public JumpStartService(MeredithDbContext dbContext, IBackgroundJobClient backgroundJobClient,
            ArticleService articleService)
        {
            _dbContext = dbContext;
            _backgroundJobClient = backgroundJobClient;
            _articleService = articleService;
        }

        public async Task Confirm(int jumpStartId, DateTime dateTime, List<string> distributionGroups,
            List<int> articleIds)
        {
            if (articleIds.Count > MaximumArticlesPerDayCount)
            {
                throw new InvalidActionException($"Maximum {MaximumArticlesPerDayCount} articles are allowed per email");
            }

            var jumpStart = await _dbContext.JumpStarts
                .Include(item => item.Articles)
                .FirstOrDefaultAsync(item => item.Id == jumpStartId);

            // Move articles that are removed from this JumpStart to tomorrow
            var removedArticles = jumpStart.Articles.Where(item => !articleIds.Contains(item.Id)).ToList();
            await MoveArticlesToTomorrow(removedArticles);

            var newArticles = await _dbContext.Articles.Where(item => articleIds.Contains(item.Id))
                .ToListAsync();

            jumpStart.DateTime = dateTime;
            jumpStart.DistributionGroups = string.Join(',', distributionGroups);
            jumpStart.Articles = newArticles;
            jumpStart.Status = JumpStartStatus.ReadyToSend;
            _dbContext.JumpStarts.Update(jumpStart);

            foreach (var article in newArticles)
            {
                article.JumpStart = jumpStart;
                article.Order = articleIds.IndexOf(article.Id);
            }

            _dbContext.UpdateRange(newArticles);

            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Enqueue<JumpStartPdfService>(service =>
                service.CreatePdfAsync(jumpStart.Id));
        }

        private async Task MoveArticlesToTomorrow(List<Article> articles)
        {
            foreach (var article in articles)
            {
                await _articleService.EditAsync(article.Id, article.CategoryId, article.Date.AddDays(1),
                    article.Headline, article.Description, article.Price, article.EventDate);
            }
        }
    }
}