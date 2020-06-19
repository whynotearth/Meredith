using System;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class NewJumpStartSingleStats : NewJumpStartOverAllStats
    {
        public int RecipientsCount { get; }

        public int DeliverPercent { get; }

        public DateTime? FirstDeliverDateTime { get; }

        public DateTime? LastOpenDateTime { get; }

        public DateTime? LastClickDateTime { get; }

        public NewJumpStartSingleStats(int recipientsCount, int deliverPercent, DateTime? firstDeliverDateTime,
            DateTime? lastOpenDateTime, DateTime? lastClickDateTime, NewJumpStartOverAllStats newJumpStartOverAllStats)
            : base(newJumpStartOverAllStats.UserCountBeforeStart, newJumpStartOverAllStats.Users,
                newJumpStartOverAllStats.OpenCountBeforeStart, newJumpStartOverAllStats.Opens,
                newJumpStartOverAllStats.ClickCountBeforeStart, newJumpStartOverAllStats.Clicks,
                newJumpStartOverAllStats.Tags)
        {
            RecipientsCount = recipientsCount;
            DeliverPercent = deliverPercent;
            FirstDeliverDateTime = firstDeliverDateTime;
            LastOpenDateTime = lastOpenDateTime;
            LastClickDateTime = lastClickDateTime;
        }
    }
}