using System;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class OverAllStats
    {
        public int UserCountBeforeStart { get; }

        public List<DailyStats> Users { get; }

        public int OpenCountBeforeStart { get; }

        public List<DailyStats> Opens { get; }

        public int ClickCountBeforeStart { get; }

        public List<DailyStats> Clicks { get; }

        public OverAllStats(int userCountBeforeStart, List<DailyStats> users, int openCountBeforeStart, List<DailyStats> opens, int clickCountBeforeStart, List<DailyStats> clicks)
        {
            UserCountBeforeStart = userCountBeforeStart;
            Users = users;
            OpenCountBeforeStart = openCountBeforeStart;
            Opens = opens;
            ClickCountBeforeStart = clickCountBeforeStart;
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