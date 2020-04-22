using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class MemoInfo
    {
        public Memo Memo { get; }

        public int OpenPercentage { get; }

        public MemoInfo(Memo memo, int openPercentage)
        {
            Memo = memo;
            OpenPercentage = openPercentage;
        }
    }
}