using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class NewJumpStartStatsResult
    {
        public int UserCount { get; }

        public int UserGrowthPercent { get; } = 100;

        public int OpenCount { get; }

        public int OpenGrowthPercent { get; } = 100;

        public int ClickCount { get; }

        public int ClickGrowthPercent { get; } = 100;

        public List<JumpStartDailyStatsResult> Users { get; }

        public List<JumpStartDailyStatsResult> Opens { get; }

        public List<JumpStartDailyStatsResult> Clicks { get; }

        public List<JumpStartDailyTagStatsResult> Tags { get; }

        //public List<JumpStartHeatMapResult> HeatMap { get; set; } = new List<JumpStartHeatMapResult>();

        public NewJumpStartStatsResult(NewJumpStartOverAllStats newJumpStartOverAllStats)
        {
            Users = newJumpStartOverAllStats.Users.Select(item => new JumpStartDailyStatsResult(item)).ToList();
            Opens = newJumpStartOverAllStats.Opens.Select(item => new JumpStartDailyStatsResult(item)).ToList();
            Clicks = newJumpStartOverAllStats.Clicks.Select(item => new JumpStartDailyStatsResult(item)).ToList();
            Tags = newJumpStartOverAllStats.Tags.Select(item => new JumpStartDailyTagStatsResult(item)).ToList();
            
            UserCount = Users.LastOrDefault()?.Count ?? 0;
            OpenCount = Opens.LastOrDefault()?.Count ?? 0;
            ClickCount = Clicks.LastOrDefault()?.Count ?? 0;

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

    public class JumpStartDailyStatsResult
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; }

        public int Count { get; }

        public JumpStartDailyStatsResult(DailyStats dailyStats)
        {
            Date = dailyStats.Date;
            Count = dailyStats.Count;
        }
    }

    public class JumpStartDailyTagStatsResult
    {
        public string Tag { get; }

        public List<JumpStartDailyStatsResult> Stats { get; }

        public JumpStartDailyTagStatsResult(NewJumpStartDailyTagStats newJumpStartDailyTagStats)
        {
            Tag = newJumpStartDailyTagStats.Tag;
            Stats = newJumpStartDailyTagStats.Stats.Select(item => new JumpStartDailyStatsResult(item)).ToList();
        }
    }

    public class JumpStartHeatMapResult
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; }

        public List<JumpStartHourlyHeatMapResult> Hours { get; }

        public JumpStartHeatMapResult(DateTime date, List<JumpStartHourlyHeatMapResult> hours)
        {
            Date = date;
            Hours = hours;
        }
    }

    public class JumpStartHourlyHeatMapResult
    {
        public int Hour { get; }

        public int Activity { get; }

        public JumpStartHourlyHeatMapResult(int hour, int activity)
        {
            Hour = hour;
            Activity = activity;
        }
    }
}