using System;
using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class MemoStatResult
    {
        public int Id { get; }

        public List<string> DistributionGroups { get; }

        public string Subject { get; }

        public DateTime CreationDateTime { get; }

        public string To { get; }

        public string Description { get; }

        // TODO: Remove these
        public int SentCount { get; }

        public int OpenCount { get; }

        public int OpenPercentage { get; }
        
        public MemoStatResult(MemoInfo memoInfo)
        {
            Id = memoInfo.Memo.Id;
            DistributionGroups = memoInfo.Memo.DistributionGroups.Split(',').ToList();
            Subject = memoInfo.Memo.Subject;
            CreationDateTime = memoInfo.Memo.CreationDateTime;
            To = memoInfo.Memo.To;
            Description = memoInfo.Memo.Description;
            OpenPercentage = memoInfo.ListStats.OpenPercentage;
        }
    }
}