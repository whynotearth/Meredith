using System;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class NewJumpStartSingleStatsResult : NewJumpStartOverAllStatsResult
    {
        public int RecipientsCount { get; }

        public int DeliverPercent { get; }

        public DateTime FirstDeliverDateTime { get; }

        public DateTime LastOpenDateTime { get; }

        public DateTime LastClickDateTime { get; }

        public NewJumpStartSingleStatsResult(NewJumpStartOverAllStats newJumpStartOverAllStats, int recipientsCount,
            int deliverPercent, DateTime firstDeliverDateTime, DateTime lastOpenDateTime, DateTime lastClickDateTime) :
            base(newJumpStartOverAllStats)
        {
            RecipientsCount = recipientsCount;
            DeliverPercent = deliverPercent;
            FirstDeliverDateTime = firstDeliverDateTime;
            LastOpenDateTime = lastOpenDateTime;
            LastClickDateTime = lastClickDateTime;
        }
    }
}