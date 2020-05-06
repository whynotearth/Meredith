using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly MeredithDbContext _dbContext;

        public JumpStartService(MeredithDbContext dbContext, IBackgroundJobClient backgroundJobClient)
        {
            _dbContext = dbContext;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task CreateAsync(DateTime dateTime, List<string> distributionGroups, List<int> postIds)
        {
            var posts = await _dbContext.Posts.Where(item => postIds.Contains(item.Id))
                .ToListAsync();

            var jumpStart = new JumpStart
            {
                DateTime = dateTime,
                DistributionGroups = string.Join(',', distributionGroups),
                Status = JumpStartStatus.ReadyToSend,
                Posts = posts
            };

            foreach (var post in posts)
            {
                post.JumpStart = jumpStart;
                post.Order = postIds.IndexOf(post.Id);
            }

            _dbContext.UpdateRange(posts);

            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Enqueue<JumpStartPdfService>(service =>
                service.CreatePdfAsync(jumpStart.Id));
        }
    }
}