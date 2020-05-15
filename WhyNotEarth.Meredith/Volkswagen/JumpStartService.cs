using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly JumpStartPlanService _jumpStartPlanService;

        public JumpStartService(MeredithDbContext dbContext, JumpStartPlanService jumpStartPlanService)
        {
            _dbContext = dbContext;
            _jumpStartPlanService = jumpStartPlanService;
        }

        public async Task<JumpStart> CreateOrEditAsync(int? jumpStartId, DateTime dateTime, List<string> distributionGroups,
            List<int> articleIds)
        {
            var articles = await GetArticles(articleIds);

            return await CreateOrEditAsync(jumpStartId, dateTime, distributionGroups, articles);
        }

        public Task<JumpStart> CreateOrEditAsync(int? jumpStartId, DateTime dateTime, List<string> distributionGroups,
            List<Article> articles)
        {
            if (articles.Count > _jumpStartPlanService.MaximumArticlesPerDayCount)
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
                return EditAsync(jumpStartId.Value, dateTime, distributionGroups, articles);
            }
            
            return CreateAsync(dateTime, distributionGroups, articles);
        }
        
        private async Task<JumpStart> CreateAsync(DateTime dateTime, List<string> distributionGroups, List<Article> articles)
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

        private async Task<JumpStart> EditAsync(int jumpStartId, DateTime dateTime, List<string> distributionGroups,
            List<Article> articles)
        {
            var jumpStart = await GetAsync(jumpStartId);

            jumpStart.DateTime = dateTime;
            jumpStart.DistributionGroups = string.Join(',', distributionGroups);
            _dbContext.JumpStarts.Update(jumpStart);

            Rearrange(articles);

            await _dbContext.SaveChangesAsync();

            return jumpStart;
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