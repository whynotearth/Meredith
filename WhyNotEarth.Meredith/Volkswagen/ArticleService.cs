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

        public int MaximumArticlesPerDayCount { get; } = 5;

        public ArticleService(MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAsync(string categorySlug, DateTime date, string headline, string description,
            decimal? price, DateTime? eventDate, string? imageUrl)
        {
            var category = await ValidateAsync(categorySlug, date);

            var article = new Article
            {
                CategoryId = category.Id,
                Date = date,
                Headline = headline,
                Description = description,
                Price = price,
                EventDate = eventDate
            };

            if (!string.IsNullOrEmpty(imageUrl))
            {
                article.Image = new ArticleImage
                {
                    Url = imageUrl
                };
            }

            await EnsureJumpStartExistAsync(article.Date);

            await _dbContext.Articles.AddAsync(article);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Article> EditAsync(int articleId, string categorySlug, DateTime date, string headline,
            string description, decimal? price, DateTime? eventDate)
        {
            var category = await ValidateAsync(categorySlug, date);

            var article = await GetAsync(articleId);

            if (article.Date != date)
            {
                await RemoveOldJumpStartAsync(article);
                await EnsureJumpStartExistAsync(article.Date);
            }

            article.CategoryId = category.Id;
            article.Date = date;
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
            var article = await GetAsync(articleId);

            if (article.Image != null)
            {
                // I'm not sure why but cascade doesn't work on this
                var isUsedInAnyOtherArticle =
                    _dbContext.Articles.Any(item => item.ImageId == article.ImageId && item.Id != article.Id);

                if (!isUsedInAnyOtherArticle)
                {
                    _dbContext.Images.Remove(article.Image);
                }
            }

            if (article.JumpStart != null)
            {
                await RemoveOldJumpStartAsync(article);
            }

            _dbContext.Articles.Remove(article);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Article>> GetAvailableArticles(DateTime date)
        {
            var articles = await _dbContext.Articles
                .Include(item => item.Category)
                .ThenInclude(item => item.Image)
                .Include(item => item.Image)
                .Where(item => item.JumpStartId == null && item.Date <= date)
                .OrderBy(item => item.Date)
                .ThenByDescending(item => item.Category.Priority)
                .ToListAsync();

            return articles;
        }

        internal async Task<List<Article>> GetDefaultArticlesAsync(DateTime date)
        {
            return await _dbContext.Articles
                .Include(item => item.Category)
                .ThenInclude(item => item.Image)
                .Where(item => item.JumpStartId == null && item.Date == date)
                .OrderBy(item => item.Category.Priority)
                .Take(MaximumArticlesPerDayCount)
                .ToListAsync();
        }

        private async Task RemoveOldJumpStartAsync(Article article)
        {
            var isUsedInAnyOtherArticle = await _dbContext.Articles.AnyAsync(item =>
                item.JumpStartId == article.JumpStartId && item.Id != article.Id);

            if (!isUsedInAnyOtherArticle)
            {
                _dbContext.JumpStarts.Remove(article.JumpStart);
            }
        }

        private async Task EnsureJumpStartExistAsync(DateTime date)
        {
            var jumpStart = await _dbContext.JumpStarts.FirstOrDefaultAsync(item =>
                item.DateTime.Date == date);

            if (jumpStart != null)
            {
                if (jumpStart.Status != JumpStartStatus.Preview)
                {
                    // This should never happen
                    throw new Exception();
                }

                return;
            }

            // TODO: Get default distribution group
            var emailRecipient = await _dbContext.Recipients.FirstOrDefaultAsync();
            var distributionGroup = emailRecipient?.DistributionGroup;

            if (distributionGroup is null)
            {
                throw new InvalidActionException(
                    "Cannot find any distribution group. Please import your recipients first.");
            }

            jumpStart = new JumpStart
            {
                // TODO: Get default send time
                DateTime = date.AddHours(10).AddMinutes(14),
                Status = JumpStartStatus.Preview,
                DistributionGroups = distributionGroup
            };

            _dbContext.JumpStarts.Add(jumpStart);
        }

        private async Task<Article> GetAsync(int articleId)
        {
            var article = await _dbContext.Articles.FirstOrDefaultAsync(item => item.Id == articleId);

            if (article is null)
            {
                throw new RecordNotFoundException($"Article {articleId} not found");
            }

            return article;
        }

        private async Task<ArticleCategory> ValidateAsync(string categorySlug, DateTime date)
        {
            var category = await _dbContext.Categories.OfType<ArticleCategory>()
                .FirstOrDefaultAsync(item => item.Slug.ToLower() == categorySlug.ToLower());

            if (category is null)
            {
                throw new RecordNotFoundException($"Category {categorySlug} not found");
            }

            var jumpStart = await _dbContext.JumpStarts.FirstOrDefaultAsync(item => item.DateTime.Date == date);
            if (jumpStart != null && jumpStart.Status != JumpStartStatus.Preview)
            {
                throw new InvalidActionException($"The email of {date.ToShortDateString()} had already sent");
            }

            return category;
        }
    }
}