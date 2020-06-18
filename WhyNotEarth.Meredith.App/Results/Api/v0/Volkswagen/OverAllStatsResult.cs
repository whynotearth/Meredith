using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class OverAllStatsResult
    {
        public int UserCount { get; }

        public int UserGrowthPercent { get; } = 100;

        public int OpenCount { get; }

        public int OpenGrowthPercent { get; } = 100;

        public int ClickCount { get; }

        public int ClickGrowthPercent { get; } = 100;

        public List<DailyStatsResult> Users { get; }

        public List<DailyStatsResult> Opens { get; }

        public List<DailyStatsResult> Clicks { get; }

        public OverAllStatsResult(OverAllStats stats)
        {
            Users = stats.Users.Select(item => new DailyStatsResult(item)).ToList();
            Opens = stats.Opens.Select(item => new DailyStatsResult(item)).ToList();
            Clicks = stats.Clicks.Select(item => new DailyStatsResult(item)).ToList();
            
            UserCount = Users.LastOrDefault()?.Count ?? 0;
            OpenCount = Opens.Sum(item => item.Count);
            ClickCount = Clicks.Sum(item => item.Count);

            var firstUserCount = Users.FirstOrDefault()?.Count ?? 0;
            if (firstUserCount != 0)
            {
                UserGrowthPercent =  GetPercent(Users.LastOrDefault()?.Count ?? 0, firstUserCount);
            }

            var firstOpenCount = Opens.FirstOrDefault()?.Count ?? 0;
            if (firstOpenCount != 0)
            {
                OpenGrowthPercent = GetPercent(Opens.LastOrDefault()?.Count ?? 0, firstOpenCount);
            }

            var firstClickCount = Clicks.FirstOrDefault()?.Count ?? 0;
            if (firstClickCount != 0)
            {
                ClickGrowthPercent = GetPercent(Clicks.LastOrDefault()?.Count ?? 0, firstClickCount);
            }
        }

        private int GetPercent(double first, int last)
        {
            var difference = last - first;
            return (int) (difference / first * 100);
        }
    }

    public class DailyStatsResult
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; }

        public int Count { get; }

        public DailyStatsResult(DailyStats jumpStartDailyStats)
        {
            Date = jumpStartDailyStats.Date;
            Count = jumpStartDailyStats.Count;
        }
    }
}