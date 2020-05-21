using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Jobs.Public;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class MemoService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly MeredithDbContext _dbContext;
        private readonly EmailRecipientService _emailRecipientService;

        public MemoService(MeredithDbContext dbContext, IBackgroundJobClient backgroundJobClient,
            EmailRecipientService emailRecipientService)
        {
            _dbContext = dbContext;
            _backgroundJobClient = backgroundJobClient;
            _emailRecipientService = emailRecipientService;
        }

        public async Task CreateAsync(List<string> distributionGroups, string subject, string date, string to,
            string description)
        {
            var memo = new Memo
            {
                DistributionGroups = string.Join(',', distributionGroups),
                Subject = subject,
                Date = date,
                To = to,
                Description = description,
                CreationDateTime = DateTime.UtcNow
            };

            _dbContext.Memos.Add(memo);
            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Enqueue<EmailRecipientJob>(job =>
                job.CreateForMemo(memo.Id));
        }

        public async Task<List<MemoInfo>> GetStatsAsync()
        {
            var memos = await _dbContext.Memos.OrderByDescending(item => item.CreationDateTime).ToListAsync();

            var result = new List<MemoInfo>();

            foreach (var memo in memos)
            {
                var memoStat = await _emailRecipientService.GetMemoListStatsAsync(memo.Id);

                result.Add(new MemoInfo(memo, memoStat));
            }

            return result;
        }

        public async Task<MemoInfo> GetStatsAsync(int memoId)
        {
            var memo = await _dbContext.Memos.FirstOrDefaultAsync(item => item.Id == memoId);

            if (memo is null)
            {
                throw new RecordNotFoundException($"Memo {memoId} not found");
            }

            var memoStat = await _emailRecipientService.GetMemoListStatsAsync(memo.Id);

            return new MemoInfo(memo, memoStat);
        }
    }
}