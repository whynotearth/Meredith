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
    public class ArticleService
    {
        private readonly MeredithDbContext _dbContext;

        public ArticleService(MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAsync(int categoryId, DateTime date, string headline, string description,
            decimal? price, DateTime? eventDate, string? imageUrl)
        {
            var category = await _dbContext.Categories.OfType<ArticleCategory>()
                .FirstOrDefaultAsync(item => item.Id == categoryId);

            if (category is null)
            {
                throw new RecordNotFoundException($"Category {categoryId} not found");
            }

            var article = new Article
            {
                CategoryId = categoryId,
                Date = date,
                Headline = headline,
                Description = description,
                Price = price,
                EventDate = eventDate,
            };

            if (!string.IsNullOrEmpty(imageUrl))
            {
                article.Image = new ArticleImage
                {
                    Url = imageUrl
                };
            }

            await _dbContext.Articles.AddAsync(article);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Dictionary<DateTime, List<Article>>> GetAvailableArticles(DateTime? date)
        {
            var query = _dbContext.Articles.Where(item => item.JumpStartId == null);

            if (date != null)
            {
                query = query.Where(item => item.Date <= date);
            }

            var articles = await query.Include(item => item.Category)
                .ThenInclude(item => item.Image)
                .Include(item => item.Image)
                .OrderBy(item => item.Date)
                .ThenByDescending(item => item.Category.Priority)
                .ToListAsync();

            return GetArticlesGrouped(articles);
        }

        private Dictionary<DateTime, List<Article>> GetArticlesGrouped(List<Article> articles)
        {
            var articleGroups = articles.GroupBy(item => item.Date);
            
            var today = DateTime.UtcNow.InZone(VolkswagenCompany.TimeZoneId);

            var result = new Dictionary<DateTime, List<Article>>();
            
            foreach (var dailyArticles in articleGroups)
            {
                // Any posts before today should be in today's JumpStart
                var currentDate = dailyArticles.Key <= today ? today : dailyArticles.Key;

                if (result.ContainsKey(currentDate))
                {
                    result[currentDate].AddRange(dailyArticles);
                }
                else
                {
                    result.Add(currentDate, dailyArticles.ToList());
                }
            }

            return result;
        }

        public async Task<Article> EditAsync(int articleId, int categoryId, DateTime date, string headline,
            string description, decimal? price, DateTime? eventDate)
        {
            var article = await _dbContext.Articles.FirstOrDefaultAsync(item => item.Id == articleId);

            if (article is null)
            {
                throw new RecordNotFoundException($"Post {articleId} not found");
            }

            article.Date = date;
            article.CategoryId = categoryId;
            article.Headline = headline;
            article.Description = description;
            article.Price = price;
            article.EventDate = eventDate;

            _dbContext.Articles.Update(article);
            await _dbContext.SaveChangesAsync();

            return article;
        }

        public async Task DeleteAsync(int articleId)
        {
            var article = await _dbContext.Articles.Include(item => item.Image)
                .FirstOrDefaultAsync(item => item.Id == articleId);

            if (article is null)
            {
                throw new RecordNotFoundException($"Post {articleId} not found");
            }

            if (article.Image != null)
            {
                // I'm not sure why but cascade doesn't work on this
                var isUsedInAnyOtherPost = _dbContext.Articles.Any(item => item.ImageId == article.ImageId && item.Id != article.Id);
                if (!isUsedInAnyOtherPost)
                {
                    _dbContext.Images.Remove(article.Image);
                }
            }

            _dbContext.Articles.Remove(article);

            await _dbContext.SaveChangesAsync();
        }
    }
}