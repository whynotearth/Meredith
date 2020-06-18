using System;
using System.Collections.Generic;
using Twilio.Rest.Api.V2010.Account.Usage;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class OverAllStats
    {
        public List<DailyStats> Users { get; }

        public int OpenCountBeforeStart { get; }

        public List<DailyStats> Opens { get; }

        public int ClickCountBeforeStart { get; }

        public List<DailyStats> Clicks { get; }

        public OverAllStats(List<DailyStats> users, int openCountBeforeStart, List<DailyStats> opens, int clickCountBeforeStart, List<DailyStats> clicks)
        {
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