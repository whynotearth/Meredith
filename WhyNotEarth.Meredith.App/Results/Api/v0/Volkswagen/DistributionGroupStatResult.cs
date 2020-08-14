using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class DistributionGroupStatResult
    {
        public string DistributionGroup { get; }

        public int SubscriberCount { get; }

        public decimal OpenPercent { get; }

        public decimal ClickPercent { get; }

        public DistributionGroupStatResult(DistributionGroupStats distributionGroupStats)
        {
            DistributionGroup = distributionGroupStats.Name;
            SubscriberCount = distributionGroupStats.RecipientsCount;
            OpenPercent = distributionGroupStats.OpenPercent;
            ClickPercent = distributionGroupStats.ClickPercent;
        }
    }
}