namespace WhyNotEarth.Meredith.Volkswagen
{
    public class MemoInfo
    {
        public Memo Memo { get; }

        public ListStats ListStats { get; }

        public MemoInfo(Memo memo, ListStats listStats)
        {
            Memo = memo;
            ListStats = listStats;
        }
    }
}