namespace WhyNotEarth.Meredith.Volkswagen
{
    public class DistributionGroupStats
    {
        public string Name { get; }

        public int RecipientsCount { get; }

        public int OpenPercent { get; }

        public int ClickPercent { get; }

        public DistributionGroupStats(string name, int recipientsCount, int memoCount, int openCount, int clickCount)
        {
            Name = name;
            RecipientsCount = recipientsCount;

            if (memoCount == 0)
            {
                OpenPercent = 100;
                ClickPercent = 100;
            }
            else
            {
                var total = memoCount * RecipientsCount;
                OpenPercent = (int) ((double) openCount / total * 100);
                ClickPercent = (int) ((double) clickCount / total * 100);
            }
        }
    }
}