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

        public async Task<Dictionary<DateTime, List<Article>>> GetPlanAsync()
        {
            var today = DateTime.UtcNow.Date;
            var result = new Dictionary<DateTime, List<Article>>();

            // First add and close all the dates with created JumpStarts
            await AddJumpStarts(result);

            var articles = await _dbContext.Articles
                .Include(item => item.Category)
                .ThenInclude(item => item.Image)
                .Include(item => item.Image)
                .Where(item => item.JumpStartId == null)
                .OrderBy(item => item.Date)
                .ThenBy(item => item.Category.Priority)
                .ToListAsync();

            EnsureAllDatesExist(result, articles);

            var oldArticles = new List<Article>();
            foreach (var article in articles)
            {
                if (article.Date >= today)
                {
                    AddToFirstOpenDateFrom(result, article.Date, article);
                }
                else
                {
                    oldArticles.Add(article);
                }
            }

            AddToOpenDays(result, oldArticles);

            return result;
        }

        private async Task AddJumpStarts(Dictionary<DateTime, List<Article>> result)
        {
            var jumpStarts = await _dbContext.JumpStarts
                .Include(item => item.Articles)
                .ThenInclude(item => item.Category)
                .ThenInclude(item => item.Image)
                .Include(item => item.Articles)
                .ThenInclude(item => item.Image)
                .Where(item => item.Status == JumpStartStatus.Preview)
                .ToListAsync();

            foreach (var jumpStart in jumpStarts)
            {
                EnsureDayExist(result, jumpStart.DateTime);

                result[jumpStart.DateTime.Date].AddRange(jumpStart.Articles);
            }
        }

        private void AddToFirstOpenDateFrom(Dictionary<DateTime, List<Article>> dailyArticles, DateTime dateTime,
            Article article)
        {
            EnsureDayExist(dailyArticles, dateTime);

            if (IsDateOpen(dailyArticles, dateTime))
            {
                dailyArticles[dateTime.Date].Add(article);
            }
            else
            {
                AddToFirstOpenDateFrom(dailyArticles, dateTime.AddDays(1), article);
            }
        }

        private void AddToOpenDays(Dictionary<DateTime, List<Article>> dailyArticles, List<Article> articles)
        {
            DateTime firstDate;
            if (dailyArticles.Any())
            {
                firstDate = dailyArticles.FirstOrDefault().Key;
            }
            else
            {
                firstDate = DateTime.UtcNow.Date;
            }

            foreach (var article in articles)
            {
                AddToFirstOpenDateFrom(dailyArticles, firstDate, article);
            }
        }

        private void EnsureDayExist(Dictionary<DateTime, List<Article>> dailyArticles, DateTime dateTime)
        {
            if (dailyArticles.ContainsKey(dateTime.Date))
            {
                return;
            }

            dailyArticles.Add(dateTime.Date, new List<Article>());
        }

        private void EnsureAllDatesExist(Dictionary<DateTime, List<Article>> dailyArticles, List<Article> articles)
        {
            var lastDate = GetLastDate(dailyArticles, articles);

            foreach (var date in GetRange(DateTime.UtcNow.Date, lastDate))
            {
                EnsureDayExist(dailyArticles, date);
            }
        }

        private DateTime GetLastDate(Dictionary<DateTime, List<Article>> dailyArticles, List<Article> articles)
        {
            var jumpStartLastDate = dailyArticles.Max(item => item.Key);
            var articlesLastDate = articles.Max(item => item.Date);
            var lastDate = Max(jumpStartLastDate, articlesLastDate);

            return lastDate;
        }

        private IEnumerable<DateTime> GetRange(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1))
            {
                yield return date;
            }
        }

        private bool IsDateOpen(Dictionary<DateTime, List<Article>> dailyArticles, DateTime dateTime)
        {
            EnsureDayExist(dailyArticles, dateTime);

            var articles = dailyArticles[dateTime.Date];

            if (articles.Count >= MaximumArticlesPerDayCount)
            {
                return false;
            }

            if (articles.Any(item => item.JumpStart != null))
            {
                return false;
            }

            return true;
        }

        private DateTime Max(DateTime a, DateTime b)
        {
            return a > b ? a : b;
        }
    }
}