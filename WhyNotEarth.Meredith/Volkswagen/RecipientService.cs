using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class RecipientService
    {
        private readonly MeredithDbContext _dbContext;

        public RecipientService(MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Import(Stream stream)
        {
            await DeleteOldRecords();

            using var csv = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);

            foreach (var batch in csv.GetRecords<RecipientCsvModel>().Batch(1000))
            {
                await InsertRecords(batch);
            }
        }

        private async Task InsertRecords(IEnumerable<RecipientCsvModel> records)
        {
            var recipients = records.Select(Convert).ToList();

            _dbContext.Recipients.AddRange(recipients);

            await _dbContext.SaveChangesAsync();
        }

        private async Task DeleteOldRecords()
        {
            await _dbContext.Database.ExecuteSqlRawAsync(
                $"TRUNCATE \"{Recipient.ModuleName}\".\"{Recipient.TableName}\" RESTART IDENTITY;");
        }

        private Recipient Convert(RecipientCsvModel csvModel)
        {
            return new Recipient
            {
                Email = csvModel.EmailAddress,
                FirstName = csvModel.FirstName,
                LastName = csvModel.LastName,
                DistributionGroup = csvModel.DistributionGroup,
                CreationDateTime = DateTime.UtcNow
            };
        }

        public async Task<List<string>> GetDistributionGroups()
        {
            return await _dbContext.Recipients.Select(item => item.DistributionGroup).Distinct().ToListAsync();
        }

        public async Task<List<DistributionGroupInfo>> GetDistributionGroupStats()
        {
            var distributionGroups = await _dbContext.Recipients.GroupBy(item => item.DistributionGroup)
                .Select(g => new
                {
                    Name = g.Key,
                    SubscriberCount = g.Count()
                })
                .ToListAsync();

            var result = new List<DistributionGroupInfo>();

            foreach (var group in distributionGroups)
            {
                // TODO: Improve this, query in a loop and loading too many records. Implemented in a hurry :(
                var memoRecipients = await _dbContext.MemoRecipients
                    .Where(item => item.DistributionGroup == group.Name && item.Status >= MemoStatus.Opened)
                    .ToListAsync();

                var memoCount = memoRecipients.Select(item => item.MemoId).Distinct().Count();
                var openCount = memoRecipients.Count(item => item.Status == MemoStatus.Opened);
                var clickCount = memoRecipients.Count(item => item.Status == MemoStatus.Clicked);

                result.Add(new DistributionGroupInfo(group.Name, group.SubscriberCount, memoCount, openCount,
                    clickCount));
            }

            return result;
        }

        public async Task<List<Recipient>> GetRecipients(string distributionGroup)
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

        private class RecipientCsvModel
        {
            [Name("Email Address")] public string? EmailAddress { get; set; }

            [Name("First Name")] public string? FirstName { get; set; }

            [Name("Last Name")] public string? LastName { get; set; }

            [Name("Distribution Group")] public string? DistributionGroup { get; set; }
        }
    }
}