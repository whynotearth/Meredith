using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class MemoInfo
    {
        public Memo Memo { get; }

        public MemoListStats ListStats { get; }

        public MemoInfo(Memo memo, MemoListStats listStats)
        {
            Memo = memo;
            ListStats = listStats;
        }
    }
}