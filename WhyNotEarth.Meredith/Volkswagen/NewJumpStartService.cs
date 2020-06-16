using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Exceptions;
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

        public async Task<NewJumpStartStats> GetStatsAsync(DateTime fromDate, DateTime toDate)
        {
            var userStats = await GetUserStatsAsync(fromDate, toDate);

            var openStats = await GetOpenStatsAsync(fromDate, toDate);

            var clickStats = await GetClickStatsAsync(fromDate, toDate);

            var tagStats = await GetTagsStatsAsync(fromDate, toDate);

            return new NewJumpStartStats(userStats, openStats, clickStats, tagStats);
        }

        public async Task<List<JumpStartDailyStats>> GetUserStatsAsync(DateTime fromDate, DateTime toDate)
        {
            var result = new List<JumpStartDailyStats>();

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                result.Add(new JumpStartDailyStats(date, await _recipientService.GetCountAsync(date)));
            }

            return result;
        }

        public async Task<List<JumpStartDailyStats>> GetOpenStatsAsync(DateTime fromDate, DateTime toDate)
        {
            var result = new List<JumpStartDailyStats>();

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                result.Add(new JumpStartDailyStats(date,
                    await _emailRecipientService.GetNewJumpStartOpenCountAsync(date)));
            }

            return result;
        }

        public async Task<List<JumpStartDailyStats>> GetClickStatsAsync(DateTime fromDate, DateTime toDate)
        {
            var result = new List<JumpStartDailyStats>();

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                result.Add(new JumpStartDailyStats(date,
                    await _emailRecipientService.GetNewJumpStartClickCountAsync(date)));
            }

            return result;
        }

        private async Task<List<JumpStartDailyTagStats>> GetTagsStatsAsync(DateTime fromDate, DateTime toDate)
        {
            var result = new List<JumpStartDailyTagStats>();

            var newJumpStarts = await _dbContext.NewJumpStarts
                .Where(item => fromDate <= item.DateTime.Date && item.DateTime.Date <= toDate)
                .ToListAsync();

            var tags = newJumpStarts.SelectMany(item => item.Tags).Distinct().ToList();

            foreach (var tag in tags)
            {
                var tagStats = new List<JumpStartDailyStats>();

                for (var date = fromDate; date <= toDate; date = date.AddDays(1))
                {
                    tagStats.Add(new JumpStartDailyStats(date, GetTagUsage(newJumpStarts, tag, date)));
                }

                result.Add(new JumpStartDailyTagStats(tag, tagStats));
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

            if (!newJumpStart.Tags.Contains(tag))
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