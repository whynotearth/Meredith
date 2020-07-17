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
            return GetDetailStatsAsync(item => item.MemoId == memoId, item => item.Memo);
        }

        public Task<ListStats> GetJumpStartListStatsAsync(int jumpStartId)
        {
            return GetStatsAsync(item => item.JumpStartId == jumpStartId);
        }

        public Task<EmailDetailStats> GetJumpStartDetailStatsAsync(int jumpStartId)
        {
            return GetDetailStatsAsync(item => item.JumpStartId == jumpStartId, item => item.JumpStart);
        }

        public async Task<DistributionGroupStats> GetDistributionGroupStats(string distributionGroup,
            int currentRecipientCount)
        {
            var stats = await _dbContext.Emails
                .Where(item => item.Group == distributionGroup)
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

            return new DistributionGroupStats(distributionGroup, currentRecipientCount, receiversCount, openCount,
                clickCount);
        }

        private async Task<ListStats> GetStatsAsync(Expression<Func<Data.Entity.Models.Email, bool>> condition)
        {
            var info = await _dbContext.Emails
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

        private async Task<EmailDetailStats> GetDetailStatsAsync<TProperty>(
            Expression<Func<Data.Entity.Models.Email, bool>> condition, Expression<Func<Data.Entity.Models.Email, TProperty>> include)
        {
            var emailRecipients = await _dbContext.Emails
                .Include(include)
                .Include(item => item.Events)
                .Where(condition).ToListAsync();

            var notOpenedList = emailRecipients.Where(item => item.Status < EmailStatus.Opened).ToList();
            var openedList = emailRecipients.Where(item => item.Status >= EmailStatus.Opened).ToList();

            return new EmailDetailStats(notOpenedList, openedList);
        }

        public Task<int> GetOpenCountAsync(DateTime date, Expression<Func<Data.Entity.Models.Email, bool>> condition)
        {
            return _dbContext.Emails
                .Include(item => item.Events)
                .Where(condition)
                .SelectMany(item => item.Events)
                .CountAsync(item => item.Type == EmailEventType.Opened && item.DateTime.Date == date);
        }

        public Task<int> GetOpenCountUpToAsync(DateTime date, Expression<Func<Data.Entity.Models.Email, bool>> condition)
        {
            return _dbContext.Emails
                .Include(item => item.Events)
                .Where(condition)
                .SelectMany(item => item.Events)
                .CountAsync(item => item.Type == EmailEventType.Opened && item.DateTime.Date <= date);
        }

        public Task<int> GetClickCountAsync(DateTime date, Expression<Func<Data.Entity.Models.Email, bool>> condition)
        {
            return _dbContext.Emails
                .Include(item => item.Events)
                .Where(condition)
                .SelectMany(item => item.Events)
                .CountAsync(item => item.Type == EmailEventType.Clicked && item.DateTime.Date == date);
        }

        public Task<int> GetClickCountUpToAsync(DateTime date, Expression<Func<Data.Entity.Models.Email, bool>> condition)
        {
            return _dbContext.Emails
                .Include(item => item.Events)
                .Where(condition)
                .SelectMany(item => item.Events)
                .CountAsync(item => item.Type == EmailEventType.Clicked && item.DateTime.Date <= date);
        }
    }
}