using System;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class MemoResult
    {
        public int Id { get; }

        public string Subject { get; }

        public DateTime CreationDateTime { get; }

        public string To { get; }

        public string Description { get; }

        public int OpenPercentage { get; }

        public MemoResult(MemoInfo memoInfo)
        {
            Id = memoInfo.Memo.Id;
            Subject = memoInfo.Memo.Subject;
            CreationDateTime = memoInfo.Memo.CreationDateTime;
            To = memoInfo.Memo.To;
            Description = memoInfo.Memo.Description;
            OpenPercentage = memoInfo.OpenPercentage;
        }
    }
}