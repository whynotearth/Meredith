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

        public async Task CreateAsync(int categoryId, DateTime date, string headline, string description,
            decimal? price, DateTime? eventDate, string? imageUrl)
        {
            var category = await _dbContext.Categories.OfType<PostCategory>()
                .FirstOrDefaultAsync(item => item.Id == categoryId);

            if (category is null)
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
            };

            if (!string.IsNullOrEmpty(imageUrl))
            {
                post.Image = new PostImage
                {
                    Url = imageUrl
                };
            }

            await _dbContext.Posts.AddAsync(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Post>> GetAvailablePosts(DateTime date)
        {
            var posts = await _dbContext.Posts
                .Include(item => item.Category)
                .ThenInclude(item => item.Image)
                .Include(item => item.Image)
                .Where(item => item.Date <= date && item.JumpStartId == null)
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

            post.Date = date;
            post.CategoryId = categoryId;
            post.Headline = headline;
            post.Description = description;
            post.Price = price;
            post.EventDate = eventDate;

            _dbContext.Posts.Update(post);
            await _dbContext.SaveChangesAsync();

            return post;
        }

        public async Task DeleteAsync(int postId)
        {
            var post = await _dbContext.Posts.Include(item => item.Image)
                .FirstOrDefaultAsync(item => item.Id == postId);

            if (post is null)
            {
                throw new RecordNotFoundException($"Post {postId} not found");
            }

            if (post.Image != null)
            {
                // I'm not sure why but cascade doesn't work on this
                var isUsedInAnyOtherPost = _dbContext.Posts.Any(item => item.ImageId == post.ImageId && item.Id != post.Id);
                if (!isUsedInAnyOtherPost)
                {
                    _dbContext.Images.Remove(post.Image);
                }
            }

            _dbContext.Posts.Remove(post);

            await _dbContext.SaveChangesAsync();
        }
    }
}