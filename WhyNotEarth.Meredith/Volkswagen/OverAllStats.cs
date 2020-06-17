using System;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class OverAllStats
    {
        public List<DailyStats> Users { get; }

        public List<DailyStats> Opens { get; }

        public List<DailyStats> Clicks { get; }

        public OverAllStats(List<DailyStats> users, List<DailyStats> opens,
            List<DailyStats> clicks)
        {
            Users = users;
            Opens = opens;
            Clicks = clicks;
        }
    }

    public class DailyStats
    {
        public DateTime Date { get; }

        public int Count { get; }

        public DailyStats(DateTime date, int count)
        {
            Date = date;
            Count = count;
        }
    }
}