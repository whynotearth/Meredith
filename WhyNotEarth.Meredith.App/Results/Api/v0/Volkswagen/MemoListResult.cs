using System;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class MemoListResult
    {
        public int Id { get; }

        public string DistributionGroup { get; }

        public string Subject { get; }

        public DateTime CreationDateTime { get; }

        public string To { get; }

        public string Description { get; }

        public int SentCount { get; }

        public int OpenCount { get; }

        public int OpenPercentage { get; }
        
        public MemoListResult(MemoInfo memoInfo)
        {
            Id = memoInfo.Memo.Id;
            DistributionGroup = memoInfo.Memo.DistributionGroup;
            Subject = memoInfo.Memo.Subject;
            CreationDateTime = memoInfo.Memo.CreationDateTime;
            To = memoInfo.Memo.To;
            Description = memoInfo.Memo.Description;
            SentCount = memoInfo.ListStats.TotalCount;
            OpenCount = memoInfo.ListStats.SentCount;
            OpenPercentage = memoInfo.ListStats.OpenPercentage;
        }
    }
}