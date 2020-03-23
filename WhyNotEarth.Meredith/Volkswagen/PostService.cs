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
            List<string> imageUrls)
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
                Images = new List<PostImage>(imageUrls.Select((item, index) =>
                    new PostImage
                    {
                        Url = item,
                        Order = index
                    }
                ))
            };

            await _dbContext.Posts.AddAsync(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Post>> GetAvailablePosts(DateTime date)
        {
            var posts = await _dbContext.Posts.Where(item => item.Date <= date.AddDays(1).AddSeconds(-1) && item.JumpStartId == null)
                .ToListAsync();

            return posts;
        }
    }
}