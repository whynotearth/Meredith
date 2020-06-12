using System;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartStats
    {
        public List<JumpStartDailyStats> Users { get; }

        public List<JumpStartDailyStats> Opens { get; }

        public List<JumpStartDailyStats> Clicks { get; }

        public List<JumpStartDailyTagStats> Tags { get; }

        public JumpStartStats(List<JumpStartDailyStats> users, List<JumpStartDailyStats> opens,
            List<JumpStartDailyStats> clicks, List<JumpStartDailyTagStats> tags)
        {
            Users = users;
            Opens = opens;
            Clicks = clicks;
            Tags = tags;
        }
    }

    public class JumpStartDailyStats
    {
        public DateTime Date { get; }

        public int Count { get; }

        public JumpStartDailyStats(DateTime date, int count)
        {
            Date = date;
            Count = count;
        }
    }

    public class JumpStartDailyTagStats
    {
        public string Tag { get; }

        public List<JumpStartDailyStats> Stats { get; }

        public JumpStartDailyTagStats(string tag, List<JumpStartDailyStats> stats)
        {
            Tag = tag;
            Stats = stats;
        }
    }
}