using System;
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

        public async Task CreateAsync(string categorySlug, DateTime date, string headline, string description,
            string? excerpt, DateTime? eventDate, string? imageUrl, int? imageWidth, int? imageHeight)
        {
            var category = await ValidateAsync(categorySlug, date);

            var article = new Article
            {
                CategoryId = category.Id,
                Date = date,
                Headline = headline,
                Description = description,
                Excerpt = excerpt,
                EventDate = eventDate
            };

            await SetImageAsync(article, imageUrl, imageWidth, imageHeight);

            await _dbContext.Articles.AddAsync(article);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Article> EditAsync(int articleId, string categorySlug, DateTime date, string headline,
            string description, string? excerpt, DateTime? eventDate, string? imageUrl, int? imageWidth, int? imageHeight)
        {
            var category = await ValidateAsync(categorySlug, date);

            var article = await GetAsync(articleId);

            article.CategoryId = category.Id;
            article.Date = date;
            article.Headline = headline;
            article.Description = description;
            article.Excerpt = excerpt;
            article.EventDate = eventDate;

            await SetImageAsync(article, imageUrl, imageWidth, imageHeight);

            _dbContext.Articles.Update(article);
            await _dbContext.SaveChangesAsync();

            return article;
        }

        public async Task DeleteAsync(int articleId)
        {
            var article = await GetAsync(articleId);

            await DeleteImageAsync(article);

            _dbContext.Articles.Remove(article);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<Article> GetAsync(int articleId)
        {
            var article = await _dbContext.Articles
                .Include(item => item.Image)
                .FirstOrDefaultAsync(item => item.Id == articleId);

            if (article is null)
            {
                throw new RecordNotFoundException($"Article {articleId} not found");
            }

            var jumpStart = await _dbContext.JumpStarts.FirstOrDefaultAsync(item => item.DateTime.Date == article.Date);
            if (jumpStart != null && jumpStart.Status != JumpStartStatus.Preview)
            {
                throw new InvalidActionException($"The email of {article.Date.ToShortDateString()} had already sent");
            }

            return article;
        }

        private async Task<ArticleCategory> ValidateAsync(string categorySlug, DateTime date)
        {
            var category = await _dbContext.Categories.OfType<ArticleCategory>()
                .FirstOrDefaultAsync(item => item.Slug == categorySlug.ToLower());

            if (category is null)
            {
                throw new RecordNotFoundException($"Category {categorySlug} not found");
            }

            var jumpStart = await _dbContext.JumpStarts.FirstOrDefaultAsync(item => item.DateTime.Date == date.Date);
            if (jumpStart != null && jumpStart.Status != JumpStartStatus.Preview)
            {
                throw new InvalidActionException($"The email of {date.ToShortDateString()} had already sent");
            }

            return category;
        }

        private async Task SetImageAsync(Article article, string? imageUrl, int? imageWidth, int? imageHeight)
        {
            await DeleteImageAsync(article);

            if (!string.IsNullOrEmpty(imageUrl))
            {
                article.Image = new ArticleImage
                {
                    Url = imageUrl,
                    Width = imageWidth,
                    Height = imageHeight
                };
            }
        }

        private async Task DeleteImageAsync(Article article)
        {
            if (article.ImageId is null)
            {
                return;
            }

            var isUsedInAnyOtherArticle =
                await _dbContext.Articles.AnyAsync(item =>
                    item.ImageId == article.ImageId && item.Id != article.Id);

            if (!isUsedInAnyOtherArticle)
            {
                _dbContext.Images.Remove(article.Image!);
            }
        }
    }
}