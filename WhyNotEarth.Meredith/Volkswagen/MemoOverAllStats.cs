using System;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class MemoOverAllStats
    {
        public List<MemoDailyStats> Users { get; }

        public List<MemoDailyStats> Opens { get; }

        public List<MemoDailyStats> Clicks { get; }

        public MemoOverAllStats(List<MemoDailyStats> users, List<MemoDailyStats> opens,
            List<MemoDailyStats> clicks)
        {
            Users = users;
            Opens = opens;
            Clicks = clicks;
        }
    }

    public class MemoDailyStats
    {
        public DateTime Date { get; }

        public int Count { get; }

        public MemoDailyStats(DateTime date, int count)
        {
            Date = date;
            Count = count;
        }
    }
}