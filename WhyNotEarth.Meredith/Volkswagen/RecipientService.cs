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
                DistributionGroup = csvModel.DistributionGroup
            };
        }

        public async Task<List<string>> GetDistinctDistributionGroups()
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
                result.Add(new DistributionGroupInfo(group.Name, group.SubscriberCount, 0, 0 , 0));
            }

            return result;
        }

        private class RecipientCsvModel
        {
            [Name("Email Address")] public string? EmailAddress { get; set; }

            [Name("First Name")] public string? FirstName { get; set; }

            [Name("Last Name")] public string? LastName { get; set; }

            [Name("Distribution Group")] public string? DistributionGroup { get; set; }
        }
    }

    public class DistributionGroupInfo
    {
        public string Name { get; }

        public int SubscriberCount { get; }

        public int OpenPercent { get; }

        public int ClickPercent { get; }

        public DistributionGroupInfo(string name, int subscriberCount, int memoCount, int openCount, int clickCount)
        {
            Name = name;
            SubscriberCount = subscriberCount;

            if (memoCount == 0)
            {
                OpenPercent = 100;
                ClickPercent = 100;
            }
            else
            {
                OpenPercent = openCount / (memoCount * SubscriberCount) * 100;
                ClickPercent = clickCount / (memoCount * SubscriberCount) * 100;
            }
        }
    }
}