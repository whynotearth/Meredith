namespace WhyNotEarth.Meredith.Volkswagen
{
    public class ListStats
    {
        public int SentCount { get; }

        public int OpenPercentage { get; } = 100;

        public ListStats(int sentCount, int openCount)
        {
            SentCount = sentCount;

            if (SentCount != 0)
            {
                OpenPercentage = (int) ((double) openCount / sentCount * 100);
            }
        }
    }
}