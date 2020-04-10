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
    public class PostService
    {
        private readonly MeredithDbContext _dbContext;

        public PostService(MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Post> CreateAsync(int categoryId, DateTime date, string headline, string description,
            decimal? price, DateTime? eventDate, List<string>? imageUrls)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(item => item.Id == categoryId);

            if (!(category is PostCategory))
            {
                throw new RecordNotFoundException($"Category {categoryId} not found");
            }

            var post = new Post
            {
                CategoryId = categoryId,
                Date = date,
                Headline = headline,
                Description = description,
                Price = price,
                EventDate = eventDate,
                Images = imageUrls?.Select((item, index) =>
                    new PostImage
                    {
                        Url = item,
                        Order = index
                    }
                ).ToList()
            };

            await _dbContext.Posts.AddAsync(post);
            await _dbContext.SaveChangesAsync();

            return post;
        }

        public async Task<List<Post>> GetAvailablePosts(DateTime date)
        {
            var posts = await _dbContext.Posts
                .Include(item => item.Category)
                .Where(item => item.Date <= date.AddDays(1).AddSeconds(-1) && item.JumpStartId == null)
                .OrderByDescending(item => item.Category.Priority)
                .ToListAsync();

            return posts;
        }

        public async Task<Post> EditAsync(int postId, int categoryId, DateTime date, string headline,
            string description, decimal? price, DateTime? eventDate)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(item => item.Id == postId);

            if (post is null)
            {
                throw new RecordNotFoundException($"Post {postId} not found");
            }

            post.CategoryId = categoryId;
            post.Date = date;
            post.Headline = headline;
            post.Description = description;
            post.Price = price;
            post.EventDate = eventDate;

            await _dbContext.Posts.AddAsync(post);
            await _dbContext.SaveChangesAsync();

            return post;
        }

        public async Task DeleteAsync(int postId)
        {
            var post = await _dbContext.Posts.Include(item => item.Images)
                .FirstOrDefaultAsync(item => item.Id == postId);

            if (post is null)
            {
                throw new RecordNotFoundException($"Post {postId} not found");
            }

            _dbContext.Images.RemoveRange(post.Images);
            _dbContext.Posts.Remove(post);

            await _dbContext.SaveChangesAsync();
        }
    }
}