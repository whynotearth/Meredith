using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Services;
using WhyNotEarth.Meredith.Volkswagen.Models;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class NewJumpStartService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly IFileService _fileService;
        private readonly RecipientService _recipientService;
        private readonly EmailRecipientService _emailRecipientService;

        public NewJumpStartService(MeredithDbContext dbContext, IFileService fileService, RecipientService recipientService, EmailRecipientService emailRecipientService)
        {
            _dbContext = dbContext;
            _fileService = fileService;
            _recipientService = recipientService;
            _emailRecipientService = emailRecipientService;
        }

        public async Task CreateAsync(NewJumpStartModel model)
        {
            var jumpStart = new NewJumpStart
            {
                DateTime = model.DateTime!.Value,
                Subject = model.Subject,
                DistributionGroups = model.DistributionGroups,
                Tags = model.Tags,
                Body = model.Body,
                Status = NewJumpStartStatus.Preview
            };

            _dbContext.NewJumpStarts.Add(jumpStart);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveAttachmentAsync(DateTime date, Stream stream)
        {
            var path = GetPdfPath(date);

            await _fileService.SaveAsync(path, "application/pdf", stream);
        }

        public string GetPdfPath(DateTime dateTime)
        {
            return $"{VolkswagenCompany.Slug}/JumpStartAttachments/{dateTime.Date:yyyy_MM_dd}.pdf";
        }

        public async Task<JumpStartStats> GetStatsAsync(DateTime fromDate, DateTime toDate)
        {
            var userStats = await GetUserStatsAsync(fromDate, toDate);
            
            var openStats = await GetOpenStatsAsync(fromDate, toDate);
            
            var clickStats = await GetClickStatsAsync(fromDate, toDate);

            var tagStats = await GetTagsStatsAsync(fromDate, toDate);

            return new JumpStartStats(userStats, openStats, clickStats, tagStats);
        }

        private async Task<List<JumpStartDailyStats>> GetUserStatsAsync(DateTime fromDate, DateTime toDate)
        {
            var result = new List<JumpStartDailyStats>();

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                result.Add(new JumpStartDailyStats(date, await _recipientService.GetCountAsync(date)));
            }

            return result;
        }

        private async Task<List<JumpStartDailyStats>> GetOpenStatsAsync(DateTime fromDate, DateTime toDate)
        {
            var result = new List<JumpStartDailyStats>();

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                result.Add(new JumpStartDailyStats(date, await _emailRecipientService.GetJumpStartOpenCountAsync(date)));
            }

            return result;
        }

        private async Task<List<JumpStartDailyStats>> GetClickStatsAsync(DateTime fromDate, DateTime toDate)
        {
            var result = new List<JumpStartDailyStats>();

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                result.Add(new JumpStartDailyStats(date, await _emailRecipientService.GetJumpStartClickCountAsync(date)));
            }

            return result;
        }

        private async Task<List<JumpStartDailyTagStats>> GetTagsStatsAsync(DateTime fromDate, DateTime toDate)
        {
            var result = new List<JumpStartDailyTagStats>();

            var newJumpStarts = await _dbContext.NewJumpStarts.Where(item => fromDate <= item.DateTime.Date && item.DateTime.Date <= toDate)
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
    }
}