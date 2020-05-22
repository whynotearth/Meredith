using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Jobs.Volkswagen;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.Jobs.Public
{
    public class EmailRecipientJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly MeredithDbContext _dbContext;

        public EmailRecipientJob(MeredithDbContext dbContext, IBackgroundJobClient backgroundJobClient)
        {
            _dbContext = dbContext;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task CreateForMemo(int memoId)
        {
            // In case something went wrong and this is a retry
            await CleanForMemo(memoId);

            var company = await _dbContext.Companies.FirstOrDefaultAsync(item => item.Slug == VolkswagenCompany.Slug);
            var memo = await _dbContext.Memos.FirstOrDefaultAsync(item => item.Id == memoId);

            await Create(company.Id, memo.DistributionGroups, item =>
            {
                item.MemoId = memoId;
                return item;
            });

            _backgroundJobClient.Enqueue<MemoJob>(job => job.SendAsync(memo.Id));
        }

        public async Task CreateForJumpStart(int jumpStartId)
        {
            // In case something went wrong and this is a retry
            await CleanForJumpStartAsync(jumpStartId);

            var company = await _dbContext.Companies.FirstOrDefaultAsync(item => item.Slug == VolkswagenCompany.Slug);
            var jumpStart = await _dbContext.JumpStarts.FirstOrDefaultAsync(item => item.Id == jumpStartId);

            await Create(company.Id, jumpStart.DistributionGroups, item =>
            {
                item.JumpStartId = jumpStartId;
                return item;
            });

            _backgroundJobClient.Enqueue<JumpStartEmailJob>(job => job.SendAsync(jumpStartId));
        }

        private async Task Create(int companyId, string distributionGroups, Func<EmailRecipient, EmailRecipient> keySetter)
        {
            var dateTime = DateTime.UtcNow;
            var recipients = await GetRecipients(distributionGroups);

            foreach (var batch in recipients.Batch(100))
            {
                var memoRecipients = batch.Select(item => new EmailRecipient
                {
                    CompanyId = companyId,
                    Email = item.Email,
                    DistributionGroup = item.DistributionGroup,
                    Status = EmailStatus.ReadyToSend,
                    CreationDateTime = dateTime
                }).Select(keySetter);

                _dbContext.EmailRecipients.AddRange(memoRecipients);
                await _dbContext.SaveChangesAsync();
            }
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

        private async Task CleanForJumpStartAsync(int jumpStartId)
        {
            var oldRecords = await _dbContext.EmailRecipients.Where(item => item.JumpStartId == jumpStartId)
                .ToListAsync();

            _dbContext.EmailRecipients.RemoveRange(oldRecords);

            await _dbContext.SaveChangesAsync();
        }
    }
}