using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Volkswagen.Models;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class RecipientService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly EmailRecipientService _emailRecipientService;

        public RecipientService(MeredithDbContext dbContext, EmailRecipientService emailRecipientService)
        {
            _dbContext = dbContext;
            _emailRecipientService = emailRecipientService;
        }

        public async Task ImportAsync(Stream stream)
        {
            var hasToCleanOldRecords = true;

            try
            {
                using var csv = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);

                foreach (var batch in csv.GetRecords<RecipientCsvModel>().Batch(1000))
                {
                    // We are doing this here so we can be sure we are able to import the new file before deleting the old records
                    if (hasToCleanOldRecords)
                    {
                        await DeleteOldRecordsAsync();
                        hasToCleanOldRecords = false;
                    }

                    await InsertRecordsAsync(batch);
                }
            }
            catch (HeaderValidationException)
            {
                throw new InvalidActionException("Invalid CSV file structure");
            }
        }

        public async Task<List<string>> GetDistributionGroupsAsync()
        {
            return await _dbContext.Recipients.Select(item => item.DistributionGroup).Distinct().ToListAsync();
        }

        public async Task<List<DistributionGroupStats>> GetDistributionGroupStatsAsync()
        {
            var distributionGroups = await _dbContext.Recipients.GroupBy(item => item.DistributionGroup)
                .Select(g => new
                {
                    Name = g.Key,
                    RecipientCount = g.Count()
                })
                .ToListAsync();

            var result = new List<DistributionGroupStats>();

            foreach (var group in distributionGroups)
            {
                var stats = await _emailRecipientService.GetDistributionGroupStats(group.Name, group.RecipientCount);

                result.Add(stats);
            }

            return result;
        }

        public async Task<List<Recipient>> GetRecipientsAsync(string distributionGroup)
        {
            return await _dbContext.Recipients.Where(item => item.DistributionGroup == distributionGroup).ToListAsync();
        }

        public Task AddAsync(string distributionGroup, RecipientModel model)
        {
            var hasDuplicate = _dbContext.Recipients.Any(item =>
                item.DistributionGroup == distributionGroup && item.Email == model.Email);

            if (hasDuplicate)
            {
                throw new DuplicateRecordException($"The email {model.Email} already exist in {distributionGroup}");
            }

            _dbContext.Recipients.Add(new Recipient
            {
                DistributionGroup = distributionGroup,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreationDateTime = DateTime.UtcNow
            });

            return _dbContext.SaveChangesAsync();
        }

        public async Task EditAsync(int recipientId, RecipientModel model)
        {
            var recipient = await _dbContext.Recipients.FirstOrDefaultAsync(item => item.Id == recipientId);

            if (recipient is null)
            {
                throw new RecordNotFoundException($"Recipient {recipientId} not found");
            }

            var hasDuplicate = _dbContext.Recipients.Any(item =>
                item.Id != recipient.Id && item.DistributionGroup == recipient.DistributionGroup &&
                item.Email == model.Email);

            if (hasDuplicate)
            {
                throw new DuplicateRecordException(
                    $"The email {model.Email} already exist in {recipient.DistributionGroup}");
            }

            recipient.Email = model.Email;
            recipient.FirstName = model.FirstName;
            recipient.LastName = model.LastName;

            _dbContext.Update(recipient);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int recipientId)
        {
            var recipient = await _dbContext.Recipients.FirstOrDefaultAsync(item => item.Id == recipientId);

            if (recipient is null)
            {
                throw new RecordNotFoundException($"Recipient {recipientId} not found");
            }

            _dbContext.Remove(recipient);
            await _dbContext.SaveChangesAsync();
        }

        public Task<int> GetUserCountAsync(DateTime date, Expression<Func<Recipient, bool>> condition)
        {
            var query = _dbContext.Recipients.Where(condition);

            return query.CountAsync(item => item.CreationDateTime.Date <= date);
        }

        private async Task InsertRecordsAsync(IEnumerable<RecipientCsvModel> records)
        {
            var recipients = records.Select(Convert).ToList();

            _dbContext.Recipients.AddRange(recipients);

            await _dbContext.SaveChangesAsync();
        }

        private async Task DeleteOldRecordsAsync()
        {
            await _dbContext.Database.ExecuteSqlRawAsync(
                $"TRUNCATE \"{Recipient.ModuleName}\".\"{Recipient.TableName}\";");
        }

        private Recipient Convert(RecipientCsvModel csvModel)
        {
            return new Recipient
            {
                Email = csvModel.EmailAddress.ToLower(),
                FirstName = csvModel.FirstName,
                LastName = csvModel.LastName,
                DistributionGroup = csvModel.DistributionGroup,
                CreationDateTime = DateTime.UtcNow
            };
        }

        public async Task<OverAllStats> GetStatsAsync(DateTime fromDate, DateTime toDate, string group)
        {
            var userStats = await GetUserStatsAsync(fromDate, toDate, group);

            var openCountBeforeStart =
                await _emailRecipientService.GetOpenCountAsync(fromDate.AddDays(-1), item => item.DistributionGroup == group);
            var openStats = await GetOpenStatsAsync(fromDate, toDate, group);

            var clickCountBeforeStart =
                await _emailRecipientService.GetOpenCountAsync(fromDate.AddDays(-1), item => item.DistributionGroup == group);
            var clickStats = await GetClickStatsAsync(fromDate, toDate, group);

            return new OverAllStats(userStats, openCountBeforeStart, openStats, clickCountBeforeStart, clickStats);
        }

        public async Task<List<DailyStats>> GetUserStatsAsync(DateTime fromDate, DateTime toDate, string group)
        {
            var result = new List<DailyStats>();

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                result.Add(new DailyStats(date, await GetUserCountAsync(date, item => item.DistributionGroup == group)));
            }

            return result;
        }

        public async Task<List<DailyStats>> GetOpenStatsAsync(DateTime fromDate, DateTime toDate, string group)
        {
            var result = new List<DailyStats>();

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                result.Add(new DailyStats(date,
                    await _emailRecipientService.GetOpenCountAsync(date, item => item.DistributionGroup == group)));
            }

            return result;
        }

        public async Task<List<DailyStats>> GetClickStatsAsync(DateTime fromDate, DateTime toDate, string group)
        {
            var result = new List<DailyStats>();

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                result.Add(new DailyStats(date,
                    await _emailRecipientService.GetClickCountAsync(date, item => item.DistributionGroup == group)));
            }

            return result;
        }

        [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
        private class RecipientCsvModel
        {
            [Name("Email Address")]
            public string EmailAddress { get; set; } = null!;

            [Name("First Name")]
            public string FirstName { get; set; } = null!;

            [Name("Last Name")]
            public string LastName { get; set; } = null!;

            [Name("Distribution Group")]
            public string DistributionGroup { get; set; } = null!;
        }
    }
}