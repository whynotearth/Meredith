using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

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

        public async Task CreateAsync(DateTime dateTime, List<string> distributionGroups, List<int> articleIds)
        {
            var articles = await _dbContext.Articles.Where(item => articleIds.Contains(item.Id))
                .ToListAsync();

            var jumpStart = new JumpStart
            {
                DateTime = dateTime,
                DistributionGroups = string.Join(',', distributionGroups),
                Status = JumpStartStatus.ReadyToSend,
                Articles = articles
            };

            foreach (var article in articles)
            {
                article.JumpStart = jumpStart;
                article.Order = articleIds.IndexOf(article.Id);
            }

            _dbContext.UpdateRange(articles);

            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Enqueue<JumpStartPdfService>(service =>
                service.CreatePdfAsync(jumpStart.Id));
        }
    }
}