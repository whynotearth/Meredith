namespace WhyNotEarth.Meredith.Volkswagen
{
    public class DistributionGroupStats
    {
        public string Name { get; }

        public int RecipientsCount { get; }

        public int OpenPercent { get; } = 100;

        public int ClickPercent { get; } = 100;

        public DistributionGroupStats(string name, int currentRecipientsCount, int receiversCount, int openCount,
            int clickCount)
        {
            Name = name;
            RecipientsCount = currentRecipientsCount;

            if (receiversCount != 0)
            {
                OpenPercent = (int)((double)openCount / receiversCount * 100);
                ClickPercent = (int)((double)clickCount / receiversCount * 100);
            }
        }
    }
}