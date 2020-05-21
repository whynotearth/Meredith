using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.Email
{
    public class EmailRecipientService
    {
        private readonly MeredithDbContext _dbContext;

        public EmailRecipientService(MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ListStats> GetMemoListStatsAsync(int memoId)
        {
            return GetStatsAsync(item => item.MemoId == memoId);
        }

        public Task<EmailDetailStats> GetMemoDetailStatsAsync(int memoId)
        {
            return GetMemoDetailStatsAsync(item => item.MemoId == memoId);
        }

        public Task<ListStats> GetJumpStartListStatsAsync(int jumpStartId)
        {
            return GetStatsAsync(item => item.JumpStartId == jumpStartId);
        }

        public Task<EmailDetailStats> GetJumpStartDetailStatsAsync(int jumpStartId)
        {
            return GetMemoDetailStatsAsync(item => item.JumpStartId == jumpStartId);
        }
        
        public async Task<DistributionGroupStats> GetDistributionGroupStats(string distributionGroup, int currentRecipientCount)
        {
            var stats = await _dbContext.EmailRecipients
                .Where(item => item.DistributionGroup == distributionGroup)
                .GroupBy(item => item.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var receiversCount = stats.Sum(item => item.Count);
            var openCount = stats.Where(item => item.Status >= EmailStatus.Opened).Sum(item => item.Count);
            var clickCount = stats.Where(item => item.Status >= EmailStatus.Clicked).Sum(item => item.Count);

            return new DistributionGroupStats(distributionGroup, currentRecipientCount, receiversCount, openCount, clickCount);
        }

        private async Task<ListStats> GetStatsAsync(Expression<Func<EmailRecipient, bool>> condition)
        {
            var info = await _dbContext.EmailRecipients
                .Where(condition)
                .GroupBy(item => item.Status)
                .Select(g => new
                {
                    g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var sentCount = info.Sum(item => item.Count);
            var openCount = info.Where(item => item.Key >= EmailStatus.Opened).Sum(item => item.Count);

            return new ListStats(sentCount, openCount);
        }

        private async Task<EmailDetailStats> GetMemoDetailStatsAsync(Expression<Func<EmailRecipient, bool>> condition)
        {
            var memoRecipients = await _dbContext.EmailRecipients.Include(item => item.Memo)
                .Where(condition).ToListAsync();

            var notOpenedList = memoRecipients.Where(item => item.Status < EmailStatus.Opened).ToList();
            var openedList = memoRecipients.Where(item => item.Status >= EmailStatus.Opened).ToList();

            return new EmailDetailStats(notOpenedList, openedList);
        }
    }
}