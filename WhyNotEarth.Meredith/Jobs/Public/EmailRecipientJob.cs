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
using WhyNotEarth.Meredith.Volkswagen.Jobs;

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

        public async Task CreateForNewJumpStart(int newJumpStartId)
        {
            // In case something went wrong and this is a retry
            await CleanForNewJumpStartAsync(newJumpStartId);

            var company = await _dbContext.Companies.FirstOrDefaultAsync(item => item.Slug == VolkswagenCompany.Slug);
            var jumpStart = await _dbContext.NewJumpStarts.FirstOrDefaultAsync(item => item.Id == newJumpStartId);

            await Create(company.Id, jumpStart.DistributionGroups, item =>
            {
                item.NewJumpStartId = newJumpStartId;
                return item;
            });

            _backgroundJobClient.Enqueue<NewJumpStartEmailJob>(job => job.SendAsync(newJumpStartId));
        }

        private async Task Create(int companyId, List<string> distributionGroups, Func<Data.Entity.Models.Email, Data.Entity.Models.Email> keySetter)
        {
            var dateTime = DateTime.UtcNow;
            var recipients = await GetRecipients(distributionGroups);

            foreach (var batch in recipients.Batch(100))
            {
                var memoRecipients = batch.Select(item => new Data.Entity.Models.Email
                {
                    CompanyId = companyId,
                    EmailAddress = item.Email,
                    Group = item.DistributionGroup,
                    Status = EmailStatus.ReadyToSend,
                    CreationDateTime = dateTime
                }).Select(keySetter);

                _dbContext.Emails.AddRange(memoRecipients);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task<List<Recipient>> GetRecipients(List<string> distributionGroups)
        {
            return await _dbContext.Recipients
                .Where(item => distributionGroups.Contains(item.DistributionGroup)).ToListAsync();
        }

        private async Task CleanForMemo(int memoId)
        {
            var oldRecords = await _dbContext.Emails.Where(item => item.MemoId == memoId).ToListAsync();

            _dbContext.Emails.RemoveRange(oldRecords);

            await _dbContext.SaveChangesAsync();
        }

        private async Task CleanForJumpStartAsync(int jumpStartId)
        {
            var oldRecords = await _dbContext.Emails.Where(item => item.JumpStartId == jumpStartId)
                .ToListAsync();

            _dbContext.Emails.RemoveRange(oldRecords);

            await _dbContext.SaveChangesAsync();
        }


        private async Task CleanForNewJumpStartAsync(int newJumpStartId)
        {
            var oldRecords = await _dbContext.Emails.Where(item => item.NewJumpStartId == newJumpStartId)
                .ToListAsync();

            _dbContext.Emails.RemoveRange(oldRecords);

            await _dbContext.SaveChangesAsync();
        }
    }
}