using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartPlanService
    {
        private readonly MeredithDbContext _dbContext;

        public int MaximumArticlesPerDayCount { get; } = 5;

        public JumpStartPlanService(MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<JumpStartPlan>> GetAsync()
        {
            var result = new List<JumpStartPlan>();
            var defaultDistributionGroups = await GetDefaultDistributionGroupsAsync();

            var articles = await GetArticlesAsync();

            var jumpStarts = await _dbContext.JumpStarts
                .OrderBy(item => item.DateTime)
                .Where(item => item.Status == JumpStartStatus.Preview)
                .ToListAsync();

            var dailyArticleGroups = articles.GroupBy(item => item.Date);
            foreach (var dailyArticles in dailyArticleGroups)
            {
                var jumpStart = GetJumpStart(jumpStarts, dailyArticles.Key);
                var sendDateTime = GetSendDateTime(jumpStart, dailyArticles.Key);
                var distributionGroups = GetDistributionGroups(jumpStart, defaultDistributionGroups);

                var jumpStartPlan = new JumpStartPlan(sendDateTime, dailyArticles.ToList(), distributionGroups, jumpStart);
                result.Add(jumpStartPlan);
            }

            return result;
        }

        private List<string> GetDistributionGroups(JumpStart? jumpStart, List<string> defaultDistributionGroups)
        {
            if (jumpStart != null)
            {
                return jumpStart.DistributionGroups.Split(',').ToList();
            }

            return defaultDistributionGroups;
        }

        private DateTime GetSendDateTime(JumpStart? jumpStart, DateTime articlesDate)
        {
            if (jumpStart != null)
            {
                return jumpStart.DateTime;
            }

            var defaultTime = GetDefaultSendTime();

            return articlesDate.Date.Add(defaultTime);
        }

        private JumpStart? GetJumpStart(List<JumpStart> jumpStarts, DateTime dateTime)
        {
            return jumpStarts.FirstOrDefault(item => item.DateTime.Date == dateTime.Date);
        }

        private async Task<List<Article>> GetArticlesAsync()
        {
            var lastSentJumpStart = await _dbContext.JumpStarts
                .OrderBy(item => item.DateTime)
                .Where(item => item.Status != JumpStartStatus.Preview)
                .LastOrDefaultAsync();

            var startDateTime = lastSentJumpStart?.DateTime ?? DateTime.MinValue;

            var articles = await _dbContext.Articles
                .Include(item => item.Category)
                .ThenInclude(item => item.Image)
                .Include(item => item.Image)
                .Where(item => item.Date > startDateTime)
                .OrderBy(item => item.Date)
                .ThenByDescending(item => item.Order)
                .ToListAsync();

            return articles;
        }

        private TimeSpan GetDefaultSendTime()
        {
            // TODO: Get default send time
            return new TimeSpan(10, 14, 0);
        }

        private async Task<List<string>> GetDefaultDistributionGroupsAsync()
        {
            var emailRecipient = await _dbContext.Recipients.FirstOrDefaultAsync();
            var distributionGroup = emailRecipient?.DistributionGroup ?? string.Empty;

            return distributionGroup.Split(',').ToList();
        }
    }
}