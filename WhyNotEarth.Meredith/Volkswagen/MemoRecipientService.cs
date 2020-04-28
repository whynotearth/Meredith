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
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly MeredithDbContext _dbContext;

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

        public async Task<DistributionGroupStats> GetDistributionGroupStats(string distributionGroup,
            int recipientCount)
        {
            var stats = await _dbContext.MemoRecipients
                .Where(item => item.DistributionGroup == distributionGroup)
                .GroupBy(item => item.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var total = stats.Sum(item => item.Count);
            var openCount = stats.Where(item => item.Status >= MemoStatus.Opened).Sum(item => item.Count);
            var clickCount = stats.Where(item => item.Status >= MemoStatus.Clicked).Sum(item => item.Count);

            return new DistributionGroupStats(distributionGroup, recipientCount, total, openCount, clickCount);
        }

        public async Task<MemoListStats> GetMemoListStats(int memoId)
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
            var sentCount = info.Sum(item => item.Count);

            return new MemoListStats(sentCount, openCount);
        }

        public async Task<MemoDetailStats> GetMemoDetailStats(int memoId)
        {
            var memoRecipients = await _dbContext.MemoRecipients.Include(item => item.Memo)
                .Where(item => item.MemoId == memoId).ToListAsync();

            var notOpenedList = memoRecipients.Where(item => item.Status < MemoStatus.Opened).ToList();
            var openedList = memoRecipients.Where(item => item.Status >= MemoStatus.Opened).ToList();

            return new MemoDetailStats(notOpenedList, openedList);
        }

        private async Task<List<Recipient>> GetRecipients(string distributionGroup)
        {
            return await _dbContext.Recipients
                .Where(item => item.DistributionGroup.ToLower() == distributionGroup.ToLower()).ToListAsync();
        }
    }
}