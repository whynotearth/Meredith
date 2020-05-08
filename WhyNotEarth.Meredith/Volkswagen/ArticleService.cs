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
                EventDate = eventDate
            };

            if (!string.IsNullOrEmpty(imageUrl))
            {
                article.Image = new ArticleImage
                {
                    Url = imageUrl
                };
            }

            await _dbContext.Articles.AddAsync(article);
            await SetJumpStart(article);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Article> EditAsync(int articleId, int categoryId, DateTime date, string headline,
            string description, decimal? price, DateTime? eventDate)
        {
            var article = await ValidateChange(articleId);

            // When the user changes the date of an article
            // We move the article to a new JumpStart
            if (article.Date != date)
            {
                article.Date = date;
                await SetJumpStart(article);
            }
            else
            {
                article.Date = date;
            }
            
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
            var article = await ValidateChange(articleId);

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
                RemoveOldJumpStart(article);
            }

            _dbContext.Articles.Remove(article);

            await _dbContext.SaveChangesAsync();
        }

        private async Task SetJumpStart(Article article)
        {
            if (article.JumpStartId != null)
            {
                RemoveOldJumpStart(article);
            }
            
            var currentDate = article.Date;
            JumpStart jumpStart;

            // Searching for a free date
            while (true)
            {
                jumpStart = await _dbContext.JumpStarts.Include(item => item.Articles)
                    .FirstOrDefaultAsync(item => item.DateTime.Date == currentDate);

                if (jumpStart is null)
                {
                    // This one is free lets create a new JumpStart here
                    jumpStart = await CreateDefaultJumpStart(currentDate);
                    break;
                }

                // This one is finalized lets try tomorrow's
                if (jumpStart.Status != JumpStartStatus.Preview)
                {
                    currentDate = currentDate.AddDays(1);
                    continue;
                }

                // Everything seems ok lets use this
                break;
            }

            article.JumpStart = jumpStart;
        }

        private void RemoveOldJumpStart(Article article)
        {
            var isUsedInAnyOtherArticle =
                _dbContext.Articles.Any(item => item.JumpStartId == article.JumpStartId && item.Id != article.Id);
            if (!isUsedInAnyOtherArticle)
            {
                _dbContext.JumpStarts.Remove(article.JumpStart);
            }
        }

        private async Task<JumpStart> CreateDefaultJumpStart(DateTime date)
        {
            // TODO: Get default distribution group
            var emailRecipient = await _dbContext.Recipients.FirstOrDefaultAsync();
            var distributionGroup = emailRecipient?.DistributionGroup;

            if (distributionGroup is null)
            {
                throw new InvalidActionException(
                    "Cannot find any distribution group. Please import your recipients first.");
            }

            var jumpStart = new JumpStart
            {
                // TODO: Get default send time
                DateTime = date.AddHours(10).AddMinutes(14),
                Status = JumpStartStatus.Preview,
                DistributionGroups = distributionGroup,
                Articles = new List<Article>()
            };

            _dbContext.JumpStarts.Add(jumpStart);

            return jumpStart;
        }

        private async Task<Article> ValidateChange(int articleId)
        {
            var article = await _dbContext.Articles
                .Include(item => item.JumpStart)
                .FirstOrDefaultAsync(item => item.Id == articleId);

            if (article is null)
            {
                throw new RecordNotFoundException($"Article {articleId} not found");
            }

            if (article.JumpStart.Status == JumpStartStatus.Sent)
            {
                throw new InvalidActionException($"Article {articleId} had already sent");
            }

            return article;
        }
    }
}