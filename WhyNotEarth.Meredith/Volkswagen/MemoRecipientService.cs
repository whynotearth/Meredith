using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class MemoRecipientService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public MemoRecipientService(MeredithDbContext dbContext, IBackgroundJobClient backgroundJobClient)
        {
            _dbContext = dbContext;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task CreateMemoRecipients(int memoId)
        {
            var memo = await _dbContext.Memos.FirstOrDefaultAsync(item => item.Id == memoId);
            var recipients = await GetRecipients(memo.DistributionGroup);

            // In case something went wrong and this is a retry
            var oldMemoRecipients = await _dbContext.MemoRecipients.Where(item => item.MemoId == memoId).ToListAsync();
            _dbContext.MemoRecipients.RemoveRange(oldMemoRecipients);
            await _dbContext.SaveChangesAsync();

            foreach (var batch in recipients.Batch(100))
            {
                var memoRecipients = batch.Select(item => new MemoRecipient
                {
                    MemoId = memoId,
                    Email = item.Email,
                    DistributionGroup = memo.DistributionGroup,
                    Status = MemoStatus.ReadyToSend
                });

                _dbContext.MemoRecipients.AddRange(memoRecipients);
                await _dbContext.SaveChangesAsync();
            }

            _backgroundJobClient.Enqueue<MemoService>(service =>
                service.SendEmailAsync(memo.Id));
        }

        public async Task<MemoStats> GetMemoStats(int memoId)
        {
            var memoRecipients = await _dbContext.MemoRecipients.Include(item => item.Memo)
                .Where(item => item.MemoId == memoId).ToListAsync();

            var notOpenedList = memoRecipients.Where(item => item.Status < MemoStatus.Opened).ToList();
            var openedList = memoRecipients.Where(item => item.Status >= MemoStatus.Opened).ToList();

            return new MemoStats(notOpenedList, openedList);
        }

        public async Task<DistributionGroupStats> GetDistributionGroupStats(string distributionGroup)
        {
            var memoRecipients = await _dbContext.MemoRecipients
                .Where(item => item.DistributionGroup == distributionGroup && item.Status >= MemoStatus.Opened)
                .ToListAsync();

            var memoCount = memoRecipients.Select(item => item.MemoId).Distinct().Count();
            var openCount = memoRecipients.Count(item => item.Status == MemoStatus.Opened);
            var clickCount = memoRecipients.Count(item => item.Status == MemoStatus.Clicked);

            return new DistributionGroupStats(distributionGroup, memoRecipients.Count, memoCount, openCount,
                clickCount);
        }

        public async Task<int> GetOpenPercentage(int memoId)
        {
            var info = await _dbContext.MemoRecipients
                .Where(item => item.MemoId == memoId)
                .GroupBy(item => item.Status)
                .Select(g => new
                {
                    g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var openCount = info.Where(item => item.Key >= MemoStatus.Opened).Sum(item => item.Count);
            var totalCount = info.Sum(item => item.Count);

            var openPercentage = 100;
            if (totalCount != 0)
            {
                openPercentage = (int)((double)openCount / totalCount * 100);
            }

            return openPercentage;
        }

        private async Task<List<Recipient>> GetRecipients(string distributionGroup)
        {
            return await _dbContext.Recipients
                .Where(item => item.DistributionGroup.ToLower() == distributionGroup.ToLower()).ToListAsync();
        }
    }
}