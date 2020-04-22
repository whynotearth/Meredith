namespace WhyNotEarth.Meredith.Volkswagen
{
    public class DistributionGroupInfo
    {
        public string Name { get; }

        public int SubscriberCount { get; }

        public int OpenPercent { get; }

        public int ClickPercent { get; }

        public DistributionGroupInfo(string name, int subscriberCount, int memoCount, int openCount, int clickCount)
        {
            Name = name;
            SubscriberCount = subscriberCount;

            if (memoCount == 0)
            {
                OpenPercent = 100;
                ClickPercent = 100;
            }
            else
            {
                var total = memoCount * SubscriberCount;
                OpenPercent = (int) ((double) openCount / total * 100);
                ClickPercent = (int) ((double) clickCount / total * 100);
            }
        }
    }
}