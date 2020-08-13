using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Volkswagen.Models;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class NewJumpStartService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly EmailRecipientService _emailRecipientService;
        private readonly RecipientService _recipientService;

        public NewJumpStartService(MeredithDbContext dbContext, RecipientService recipientService,
            EmailRecipientService emailRecipientService)
        {
            _dbContext = dbContext;
            _recipientService = recipientService;
            _emailRecipientService = emailRecipientService;
        }

        public async Task CreateAsync(NewJumpStartModel model)
        {
            var jumpStart = Map(new NewJumpStart(), model);

            jumpStart.Status = NewJumpStartStatus.Preview;

            _dbContext.NewJumpStarts.Add(jumpStart);
            await _dbContext.SaveChangesAsync();
        }

        public Task<NewJumpStartOverAllStats> GetStatsAsync(DateTime fromDate, DateTime toDate)
        {
            return GetStatsCoreAsync(fromDate, toDate, null);
        }

        public async Task<NewJumpStartSingleStats> GetStatsAsync(DateTime fromDate, DateTime toDate, int id)
        {
            var newJumpStartOverAllStats = await GetStatsCoreAsync(fromDate, toDate, id);

            var recipientsCount = await _dbContext.Emails.CountAsync(item => item.NewJumpStartId == id);
            double deliverCount = await _dbContext.Emails.CountAsync(item =>
                item.NewJumpStartId == id && item.Status >= EmailStatus.Delivered);

            var deliverPercent = 100;
            if (recipientsCount != 0)
            {
                deliverPercent = (int)(deliverCount / recipientsCount * 100);
            }

            var events = await _dbContext.Emails
                .Include(item => item.Events)
                .Where(item => item.NewJumpStartId == id)
                .SelectMany(item => item.Events)
                .OrderBy(item => item.DateTime)
                .ToListAsync();

            var firstDeliverDateTime = events.FirstOrDefault()?.DateTime;

            var lastOpenDateTime = events.LastOrDefault(item => item.Type == EmailEventType.Opened)?.DateTime;

            var lastClickDateTime = events.LastOrDefault(item => item.Type == EmailEventType.Clicked)?.DateTime;

            return new NewJumpStartSingleStats(recipientsCount, deliverPercent, firstDeliverDateTime, lastOpenDateTime,
                lastClickDateTime, newJumpStartOverAllStats);
        }

        private async Task<NewJumpStartOverAllStats> GetStatsCoreAsync(DateTime fromDate, DateTime toDate, int? id)
        {
            Expression<Func<Public.Email, bool>> condition;
            if (id.HasValue)
            {
                condition = item => item.NewJumpStartId == id;
            }
            else
            {
                condition = item => item.NewJumpStartId != null;
            }

            var userCountBeforeStart = await _recipientService.GetUserCountAsync(fromDate.AddDays(-1), item => true);
            var userStats = await GetUserStatsAsync(fromDate, toDate);

            var openCountBeforeStart =
                await _emailRecipientService.GetOpenCountUpToAsync(fromDate.AddDays(-1), condition);
            var openStats = await GetOpenStatsAsync(fromDate, toDate, condition);

            var clickCountBeforeStart =
                await _emailRecipientService.GetClickCountUpToAsync(fromDate.AddDays(-1), condition);
            var clickStats = await GetClickStatsAsync(fromDate, toDate, condition);

            var tagStats = await GetTagsStatsAsync(fromDate, toDate);

            return new NewJumpStartOverAllStats(userCountBeforeStart, userStats, openCountBeforeStart, openStats,
                clickCountBeforeStart, clickStats, tagStats);
        }

        private async Task<List<DailyStats>> GetUserStatsAsync(DateTime fromDate, DateTime toDate)
        {
            var result = new List<DailyStats>();

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                result.Add(new DailyStats(date, await _recipientService.GetUserCountAsync(date, item => true)));
            }

            return result;
        }

        private async Task<List<DailyStats>> GetOpenStatsAsync(DateTime fromDate, DateTime toDate,
            Expression<Func<Public.Email, bool>> condition)
        {
            var result = new List<DailyStats>();

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                result.Add(new DailyStats(date, await _emailRecipientService.GetOpenCountAsync(date, condition)));
            }

            return result;
        }

        private async Task<List<DailyStats>> GetClickStatsAsync(DateTime fromDate, DateTime toDate,
            Expression<Func<Public.Email, bool>> condition)
        {
            var result = new List<DailyStats>();

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                result.Add(new DailyStats(date, await _emailRecipientService.GetClickCountAsync(date, condition)));
            }

            return result;
        }

        private async Task<List<NewJumpStartDailyTagStats>> GetTagsStatsAsync(DateTime fromDate, DateTime toDate)
        {
            var result = new List<NewJumpStartDailyTagStats>();

            var newJumpStarts = await _dbContext.NewJumpStarts
                .Where(item => fromDate <= item.DateTime.Date && item.DateTime.Date <= toDate)
                .ToListAsync();

            var tags = newJumpStarts.SelectMany(item => item.Tags).Distinct().ToList();

            foreach (var tag in tags)
            {
                var tagStats = new List<DailyStats>();

                for (var date = fromDate; date <= toDate; date = date.AddDays(1))
                {
                    tagStats.Add(new DailyStats(date, GetTagUsage(newJumpStarts, tag, date)));
                }

                result.Add(new NewJumpStartDailyTagStats(tag, tagStats));
            }

            return result;
        }

        private int GetTagUsage(List<NewJumpStart> newJumpStarts, string tag, DateTime date)
        {
            var newJumpStart = newJumpStarts.FirstOrDefault(item => item.DateTime.Date == date);

            if (newJumpStart is null)
            {
                return 0;
            }

            if (newJumpStart.Tags?.Contains(tag) == false)
            {
                return 0;
            }

            return 1;
        }

        public Task<List<NewJumpStart>> ListAsync()
        {
            return _dbContext.NewJumpStarts.Where(item => item.Status == NewJumpStartStatus.Preview).ToListAsync();
        }

        public async Task EditAsync(int id, NewJumpStartModel model)
        {
            var newJumpStart = await _dbContext.NewJumpStarts.FirstOrDefaultAsync(item => item.Id == id);

            if (newJumpStart is null)
            {
                throw new RecordNotFoundException($"NewJumpStart {id} not found");
            }

            newJumpStart = Map(newJumpStart, model);

            _dbContext.NewJumpStarts.Update(newJumpStart);
            await _dbContext.SaveChangesAsync();
        }

        private NewJumpStart Map(NewJumpStart newJumpStart, NewJumpStartModel model)
        {
            newJumpStart.DateTime = model.DateTime!.Value;
            newJumpStart.Subject = model.Subject;
            newJumpStart.DistributionGroups = model.DistributionGroups;
            newJumpStart.Tags = model.Tags;
            newJumpStart.Body = model.Body;
            newJumpStart.PdfUrl = model.PdfUrl;

            return newJumpStart;
        }
    }
}