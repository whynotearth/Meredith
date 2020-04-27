namespace WhyNotEarth.Meredith.Volkswagen
{
    public class DistributionGroupStats
    {
        public string Name { get; }

        public int RecipientsCount { get; }

        public int OpenPercent { get; } = 100;

        public int ClickPercent { get; } = 100;

        public DistributionGroupStats(string name, int recipientsCount, int totalCount, int openCount, int clickCount)
        {
            Name = name;
            RecipientsCount = recipientsCount;

            if (totalCount != 0)
            {
                OpenPercent = (int) ((double) openCount / totalCount * 100);
                ClickPercent = (int) ((double) clickCount / totalCount * 100);
            }
        }
    }
}