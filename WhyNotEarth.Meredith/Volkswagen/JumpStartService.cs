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

        public async Task CreateAsync(DateTime dateTime, List<int> postIds)
        {
            var posts = await _dbContext.Posts.Where(item => postIds.Contains(item.Id))
                .ToListAsync();

            var isValid = posts.All(item => item.Date.Date <= dateTime.Date && item.JumpStartId == null) &&
                          posts.Count == postIds.Count;
            if (!isValid)
            {
                throw new InvalidActionException("Invalid posts");
            }

            var jumpStart = new JumpStart
            {
                DateTime = dateTime,
                Status = JumpStartStatus.ReadyToSend,
                Posts = posts
            };

            var order = 0;
            foreach (var post in posts)
            {
                post.JumpStart = jumpStart;
                post.Order = order;

                order += 1;
            }

            _dbContext.UpdateRange(posts);

            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Schedule<JumpStartEmailService>(service =>
                service.SendAsync(jumpStart.Id), DateTime.UtcNow - dateTime);

            _backgroundJobClient.Enqueue<JumpStartPdfService>(service =>
                service.CreatePdfAsync(jumpStart.Id));
        }
    }
}