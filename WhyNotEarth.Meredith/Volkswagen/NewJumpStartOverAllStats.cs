using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class NewJumpStartOverAllStats : OverAllStats
    {
        public List<NewJumpStartDailyTagStats> Tags { get; }

        public NewJumpStartOverAllStats(int userCountBeforeStart, List<DailyStats> users, int openCountBeforeStart,
            List<DailyStats> opens, int clickCountBeforeStart, List<DailyStats> clicks,
            List<NewJumpStartDailyTagStats> tags) : base(userCountBeforeStart, users, openCountBeforeStart, opens,
            clickCountBeforeStart, clicks)
        {
            Tags = tags;
        }
    }

    public class NewJumpStartDailyTagStats
    {
        public string Tag { get; }

        public List<DailyStats> Stats { get; }

        public NewJumpStartDailyTagStats(string tag, List<DailyStats> stats)
        {
            Tag = tag;
            Stats = stats;
        }
    }
}