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
    public class EmailRecipientService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly MeredithDbContext _dbContext;

        public EmailRecipientService(MeredithDbContext dbContext, IBackgroundJobClient backgroundJobClient)
        {
            _dbContext = dbContext;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task CreateForMemo(int memoId)
        {
            // In case something went wrong and this is a retry
            await CleanForMemo(memoId);

            var memo = await _dbContext.Memos.FirstOrDefaultAsync(item => item.Id == memoId);

            await Create(memo.DistributionGroups, item =>
            {
                item.MemoId = memoId;
                return item;
            });

            _backgroundJobClient.Enqueue<MemoService>(service =>
                service.SendAsync(memo.Id));
        }

        public async Task CreateForJumpStart(int jumpStartId)
        {
            // In case something went wrong and this is a retry
            await CleanForJumpStart(jumpStartId);

            var jumpStart = await _dbContext.JumpStarts.FirstOrDefaultAsync(item => item.Id == jumpStartId);

            await Create(jumpStart.DistributionGroups, item =>
            {
                item.JumpStartId = jumpStartId;
                return item;
            });

            _backgroundJobClient.Enqueue<JumpStartEmailService>(service =>
                service.SendAsync(jumpStartId));
        }

        private async Task Create(string distributionGroups, Func<EmailRecipient, EmailRecipient> keySetter)
        {
            var recipients = await GetRecipients(distributionGroups);

            foreach (var batch in recipients.Batch(100))
            {
                var memoRecipients = batch.Select(item => new EmailRecipient
                {
                    Email = item.Email,
                    DistributionGroup = item.DistributionGroup,
                    Status = EmailStatus.ReadyToSend
                }).Select(keySetter);

                _dbContext.EmailRecipients.AddRange(memoRecipients);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<DistributionGroupStats> GetDistributionGroupStats(string distributionGroup,
            int recipientCount)
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

            var total = stats.Sum(item => item.Count);
            var openCount = stats.Where(item => item.Status >= EmailStatus.Opened).Sum(item => item.Count);
            var clickCount = stats.Where(item => item.Status >= EmailStatus.Clicked).Sum(item => item.Count);

            return new DistributionGroupStats(distributionGroup, recipientCount, total, openCount, clickCount);
        }

        public async Task<MemoListStats> GetMemoListStats(int memoId)
        {
            var info = await _dbContext.EmailRecipients
                .Where(item => item.MemoId == memoId)
                .GroupBy(item => item.Status)
                .Select(g => new
                {
                    g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var openCount = info.Where(item => item.Key >= EmailStatus.Opened).Sum(item => item.Count);
            var sentCount = info.Sum(item => item.Count);

            return new MemoListStats(sentCount, openCount);
        }

        public async Task<EmailDetailStats> GetMemoDetailStats(int memoId)
        {
            var memoRecipients = await _dbContext.EmailRecipients.Include(item => item.Memo)
                .Where(item => item.MemoId == memoId).ToListAsync();

            var notOpenedList = memoRecipients.Where(item => item.Status < EmailStatus.Opened).ToList();
            var openedList = memoRecipients.Where(item => item.Status >= EmailStatus.Opened).ToList();

            return new EmailDetailStats(notOpenedList, openedList);
        }

        private async Task<List<Recipient>> GetRecipients(string distributionGroups)
        {
            var distributionGroupList = distributionGroups.ToLower().Split(',');

            return await _dbContext.Recipients
                .Where(item => distributionGroupList.Contains(item.DistributionGroup.ToLower())).ToListAsync();
        }

        private async Task CleanForMemo(int memoId)
        {
            var oldRecords = await _dbContext.EmailRecipients.Where(item => item.MemoId == memoId).ToListAsync();

            _dbContext.EmailRecipients.RemoveRange(oldRecords);

            await _dbContext.SaveChangesAsync();
        }

        private async Task CleanForJumpStart(int jumpStartId)
        {
            var oldRecords = await _dbContext.EmailRecipients.Where(item => item.JumpStartId == jumpStartId)
                .ToListAsync();

            _dbContext.EmailRecipients.RemoveRange(oldRecords);

            await _dbContext.SaveChangesAsync();
        }
    }
}