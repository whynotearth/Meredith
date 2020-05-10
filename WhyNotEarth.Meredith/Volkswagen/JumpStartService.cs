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
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly MeredithDbContext _dbContext;
        private readonly JumpStartPlanService _jumpStartPlanService;

        public JumpStartService(MeredithDbContext dbContext, 
            IBackgroundJobClient backgroundJobClient, JumpStartPlanService jumpStartPlanService)
        {
            _dbContext = dbContext;
            _backgroundJobClient = backgroundJobClient;
            _jumpStartPlanService = jumpStartPlanService;
        }

        public async Task CreateOrEditAsync(int? jumpStartId, DateTime dateTime, List<string> distributionGroups,
            List<int> articleIds)
        {
            if (articleIds.Count > _jumpStartPlanService.MaximumArticlesPerDayCount)
            {
                throw new InvalidActionException(
                    $"Maximum {_jumpStartPlanService.MaximumArticlesPerDayCount} articles are allowed per email");
            }

            if (!distributionGroups.Any())
            {
                throw new InvalidActionException("No distribution group selected");
            }

            if (jumpStartId.HasValue)
            {
                await EditAsync(jumpStartId.Value, dateTime, distributionGroups, articleIds);
            }
            else
            {
                await Create(dateTime, distributionGroups, articleIds);
            }
        }
        
        private async Task Create(DateTime dateTime, List<string> distributionGroups, List<int> articleIds)
        {
            var articles = await GetArticles(articleIds);

            await Create(dateTime, distributionGroups, articles);
        }

        private async Task<JumpStart> Create(DateTime dateTime, List<string> distributionGroups, List<Article> articles)
        {
            var jumpStart = new JumpStart
            {
                DateTime = dateTime,
                Status = JumpStartStatus.Preview,
                DistributionGroups = string.Join(',', distributionGroups)
            };

            _dbContext.JumpStarts.Add(jumpStart);

            Rearrange(articles);

            await _dbContext.SaveChangesAsync();

            return jumpStart;
        }

        private async Task EditAsync(int jumpStartId, DateTime dateTime, List<string> distributionGroups,
            List<int> articleIds)
        {
            var jumpStart = await GetAsync(jumpStartId);

            jumpStart.DateTime = dateTime;
            jumpStart.DistributionGroups = string.Join(',', distributionGroups);
            _dbContext.JumpStarts.Update(jumpStart);

            var articles = await GetArticles(articleIds);
            Rearrange(articles);

            await _dbContext.SaveChangesAsync();
        }

        public async Task SendAsync()
        {
            var dailyPlans = await _jumpStartPlanService.GetAsync();
            dailyPlans = dailyPlans.Where(item => item.DateTime < DateTime.UtcNow.AddMinutes(15)).ToList();

            var jumpStarts = new List<JumpStart>();

            foreach (var dailyPlan in dailyPlans)
            {
                var jumpStart = dailyPlan.JumpStart;
                if (jumpStart is null)
                {
                    jumpStart = await Create(dailyPlan.DateTime, dailyPlan.DistributionGroups, dailyPlan.Articles);
                }

                jumpStart.Status = JumpStartStatus.Sending;
                jumpStarts.Add(jumpStart);
            }

            _dbContext.UpdateRange(jumpStarts);
            await _dbContext.SaveChangesAsync();

            foreach (var jumpStart in jumpStarts)
            {
                _backgroundJobClient.Enqueue<JumpStartPdfJob>(service =>
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

        private async Task<List<Article>> GetArticles(List<int> articleIds)
        {
            var articles = await _dbContext.Articles.Where(item => articleIds.Contains(item.Id))
                .ToListAsync();

            return articles.OrderBy(item => articleIds.IndexOf(item.Id)).ToList();
        }

        private void Rearrange(List<Article> articles)
        {
            var i = 0;
            foreach (var article in articles)
            {
                article.Order = i++;
            }
         
            _dbContext.Articles.UpdateRange(articles);
        }
    }
}