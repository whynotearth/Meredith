using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Exceptions;

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

        public Task AddAsync(string distributionGroup, string email)
        {
            var hasDuplicate = _dbContext.Recipients.Any(item =>
                item.DistributionGroup == distributionGroup && item.Email == email);

            if (hasDuplicate)
            {
                throw new DuplicateRecordException($"The email {email} already exist in {distributionGroup}");
            }

            _dbContext.Recipients.Add(new Recipient
            {
                DistributionGroup = distributionGroup,
                Email = email,
                CreationDateTime = DateTime.UtcNow
            });

            return _dbContext.SaveChangesAsync();
        }

        public async Task EditAsync(int recipientId, string email)
        {
            var recipient = await _dbContext.Recipients.FirstOrDefaultAsync(item => item.Id == recipientId);

            if (recipient is null)
            {
                throw new RecordNotFoundException($"Recipient {recipientId} not found");
            }

            var hasDuplicate = _dbContext.Recipients.Any(item =>
                item.Id != recipient.Id && item.DistributionGroup == recipient.DistributionGroup &&
                item.Email == email);

            if (hasDuplicate)
            {
                throw new DuplicateRecordException($"The email {email} already exist in {recipient.DistributionGroup}");
            }

            recipient.Email = email;

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

        public Task<int> GetCountAsync(DateTime dateTime)
        {
            return _dbContext.Recipients.CountAsync(item => item.CreationDateTime <= dateTime);
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