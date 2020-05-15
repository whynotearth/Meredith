using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class VolkswagenSettings
    {
        public string? DistributionGroups { get; set; }

        public bool EnableAutoSend { get; set; } = true;

        public TimeSpan? SendTime { get; set; } = new TimeSpan(10, 14, 0);

        public async Task<List<string>> GetDistributionGroupAsync(MeredithDbContext dbContext)
        {
            if (!string.IsNullOrEmpty(DistributionGroups))
            {
                return DistributionGroups.Split(',').ToList();
            }

            var emailRecipient = await dbContext.Recipients.FirstOrDefaultAsync();
            var distributionGroup = emailRecipient?.DistributionGroup;

            if (distributionGroup is null)
            {
                throw new InvalidActionException(
                    "Cannot find any distribution group. Please import your recipients first.");
            }

            return new List<string> {distributionGroup};
        }
    }
}