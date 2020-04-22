using System;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class MemoResult
    {
        public string Subject { get; }

        public DateTime CreationDateTime { get; }

        public string To { get; }

        public string Description { get; }

        public int OpenRate { get; }

        public MemoResult(Memo memo, int openRate)
        {
            Subject = memo.Subject;
            CreationDateTime = memo.CreationDateTime;
            To = memo.To;
            Description = memo.Description;
            OpenRate = openRate;
        }
    }
}