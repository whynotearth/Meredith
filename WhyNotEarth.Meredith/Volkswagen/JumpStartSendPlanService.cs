using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartSendPlanService
    {
        private readonly MeredithDbContext _dbContext;

        public int MaximumArticlesPerDayCount { get; } = 5;

        public JumpStartSendPlanService(MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Dictionary<DateTime, List<Article>>> GetPlanAsync()
        {
            var today = DateTime.UtcNow.Date;

            var articles = await _dbContext.Articles
                .Include(item => item.Category)
                .ThenInclude(item => item.Image)
                .Include(item => item.Image)
                .Include(item => item.JumpStart)
                .Where(item => item.JumpStartId == null || item.Date >= today)
                .OrderBy(item => item.Category.Priority)
                .ToListAsync();

            articles = articles.Where(
                item => item.JumpStart == null || item.JumpStart.Status == JumpStartStatus.Preview).ToList();

            var result = new Dictionary<DateTime, List<Article>>();
            var oldArticles = new List<Article>();

            var firstDate = DateTime.UtcNow.Date;
            var lastDate = GetLastDate(articles, firstDate);
            EnsureAllDatesExist(result, firstDate, lastDate);

            foreach (var article in articles)
            {
                var date = GetDate(article);

                if (date >= today)
                {
                    AddToFirstDateFrom(result, date, article);
                }
                else
                {
                    oldArticles.Add(article);
                }
            }

            AddToOpenDays(result, oldArticles);

            return result;
        }

        private DateTime GetLastDate(List<Article> articles, DateTime firstDate)
        {
            var result = firstDate;
            foreach (var article in articles)
            {
                var date = GetDate(article);

                if (date > result)
                {
                    result = date;
                }
            }

            return result;
        }

        private DateTime GetDate(Article article)
        {
            var result = article.Date;

            if (article.JumpStart != null)
            {
                result = article.JumpStart.DateTime.Date;
            }

            return result;
        }

        private void AddToFirstDateFrom(Dictionary<DateTime, List<Article>> dailyArticles, DateTime dateTime,
            Article article)
        {
            EnsureDayExist(dailyArticles, dateTime);

            var articles = dailyArticles[dateTime.Date];

            if (articles.Count < MaximumArticlesPerDayCount)
            {
                dailyArticles[dateTime.Date].Add(article);
            }
            else
            {
                AddToFirstDateFrom(dailyArticles, dateTime.AddDays(1), article);
            }
        }

        private void AddToOpenDays(Dictionary<DateTime, List<Article>> dailyArticles, List<Article> articles)
        {
            var firstDate = dailyArticles.First().Key;

            foreach (var article in articles)
            {
                AddToFirstDateFrom(dailyArticles, firstDate, article);
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

        private void EnsureAllDatesExist(Dictionary<DateTime, List<Article>> dailyArticles, DateTime firstDate,
            DateTime lastDate)
        {
            foreach (var date in GetRange(firstDate, lastDate))
            {
                EnsureDayExist(dailyArticles, date);
            }
        }

        public IEnumerable<DateTime> GetRange(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1))
            {
                yield return date;
            }
        }
    }
}