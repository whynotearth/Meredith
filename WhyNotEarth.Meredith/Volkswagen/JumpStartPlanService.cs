using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartPlanService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly SettingsService _settingsService;

        public int MaximumArticlesPerDayCount { get; } = 5;

        public JumpStartPlanService(MeredithDbContext dbContext, SettingsService settingsService)
        {
            _dbContext = dbContext;
            _settingsService = settingsService;
        }

        public async Task<List<JumpStartPlan>> GetAsync()
        {
            var result = new List<JumpStartPlan>();
            var settings = await _settingsService.GetValueAsync<VolkswagenSettings>(VolkswagenCompany.Slug);
            var defaultDistributionGroups = await settings.GetDistributionGroupAsync(_dbContext);

            var articles = await GetArticlesAsync();

            var jumpStarts = await _dbContext.JumpStarts
                .OrderBy(item => item.DateTime)
                .Where(item => item.Status == JumpStartStatus.Preview)
                .ToListAsync();

            var dailyArticleGroups = articles.GroupBy(item => item.Date);
            foreach (var dailyArticles in dailyArticleGroups)
            {
                var jumpStart = GetJumpStart(jumpStarts, dailyArticles.Key);
                var sendDateTime = GetSendDateTime(jumpStart, dailyArticles.Key, settings);
                var distributionGroups = GetDistributionGroups(jumpStart, defaultDistributionGroups);

                var jumpStartPlan = new JumpStartPlan(sendDateTime, dailyArticles.OrderBy(item => item.Order).ToList(),
                    distributionGroups, jumpStart);
                
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

        private DateTime GetSendDateTime(JumpStart? jumpStart, DateTime articlesDate, VolkswagenSettings settings)
        {
            if (jumpStart != null)
            {
                return jumpStart.DateTime;
            }

            if (settings.EnableAutoSend)
            {
                return articlesDate.Date.Add(settings.SendTime!.Value);
            }

            return articlesDate;
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
    }
}