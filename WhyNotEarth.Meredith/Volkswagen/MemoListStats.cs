namespace WhyNotEarth.Meredith.Volkswagen
{
    public class MemoListStats
    {
        public int TotalCount { get; }

        public int SentCount { get; }

        public int OpenPercentage { get; } = 100;

        public MemoListStats(int totalCount, int sentCount)
        {
            TotalCount = totalCount;
            SentCount = sentCount;

            if (TotalCount != 0)
            {
                OpenPercentage = (int) ((double) SentCount / TotalCount * 100);
            }
        }
    }
}