using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Jobs.Public;
using WhyNotEarth.Meredith.Volkswagen.Models;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class MemoService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IDbContext _dbContext;
        private readonly EmailRecipientService _emailRecipientService;
        private readonly RecipientService _recipientService;

        public MemoService(IDbContext dbContext, IBackgroundJobClient backgroundJobClient,
            EmailRecipientService emailRecipientService, RecipientService recipientService)
        {
            _dbContext = dbContext;
            _backgroundJobClient = backgroundJobClient;
            _emailRecipientService = emailRecipientService;
            _recipientService = recipientService;
        }

        public async Task CreateAsync(MemoModel model)
        {
            var memo = new Memo
            {
                DistributionGroups = model.DistributionGroups,
                Subject = model.Subject,
                Date = model.Date,
                To = model.To,
                Description = model.Description,
                PdfUrl = model.PdfUrl,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Memos.Add(memo);
            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Enqueue<EmailRecipientJob>(job =>
                job.CreateForMemo(memo.Id));
        }

        public async Task<List<MemoInfo>> GetStatsAsync()
        {
            var memos = await _dbContext.Memos.OrderByDescending(item => item.CreatedAt).ToListAsync();

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

        public async Task<OverAllStats> GetStatsAsync(DateTime fromDate, DateTime toDate)
        {
            var userCountBeforeStart = await _recipientService.GetUserCountAsync(fromDate.AddDays(-1), item => true);
            var userStats = await GetUserStatsAsync(fromDate, toDate);

            var openCountBeforeStart =
                await _emailRecipientService.GetOpenCountUpToAsync(fromDate.AddDays(-1), item => item.MemoId != null);
            var openStats = await GetOpenStatsAsync(fromDate, toDate);

            var clickCountBeforeStart =
                await _emailRecipientService.GetClickCountUpToAsync(fromDate.AddDays(-1), item => item.MemoId != null);
            var clickStats = await GetClickStatsAsync(fromDate, toDate);

            return new OverAllStats(userCountBeforeStart, userStats, openCountBeforeStart, openStats,
                clickCountBeforeStart, clickStats);
        }

        public async Task<List<DailyStats>> GetUserStatsAsync(DateTime fromDate, DateTime toDate)
        {
            var result = new List<DailyStats>();

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                result.Add(new DailyStats(date, await _recipientService.GetUserCountAsync(date, item => true)));
            }

            return result;
        }

        public async Task<List<DailyStats>> GetOpenStatsAsync(DateTime fromDate, DateTime toDate)
        {
            var result = new List<DailyStats>();

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                result.Add(new DailyStats(date,
                    await _emailRecipientService.GetOpenCountAsync(date, item => item.MemoId != null)));
            }

            return result;
        }

        public async Task<List<DailyStats>> GetClickStatsAsync(DateTime fromDate, DateTime toDate)
        {
            var result = new List<DailyStats>();

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                result.Add(new DailyStats(date,
                    await _emailRecipientService.GetClickCountAsync(date, item => item.MemoId != null)));
            }

            return result;
        }
    }
}