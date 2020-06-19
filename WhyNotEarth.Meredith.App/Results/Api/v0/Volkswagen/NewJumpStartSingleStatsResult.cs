using System;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class NewJumpStartSingleStatsResult : NewJumpStartOverAllStatsResult
    {
        public int RecipientsCount { get; }

        public int DeliverPercent { get; }

        public DateTime? FirstDeliverDateTime { get; }

        public DateTime? LastOpenDateTime { get; }

        public DateTime? LastClickDateTime { get; }

        public NewJumpStartSingleStatsResult(NewJumpStartSingleStats newJumpStartSingleStats) : base(newJumpStartSingleStats)
        {
            RecipientsCount = newJumpStartSingleStats.RecipientsCount;
            DeliverPercent = newJumpStartSingleStats.DeliverPercent;
            FirstDeliverDateTime = newJumpStartSingleStats.FirstDeliverDateTime;
            LastOpenDateTime = newJumpStartSingleStats.LastClickDateTime;
            LastClickDateTime = newJumpStartSingleStats.LastClickDateTime;
        }
    }
}