using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class JumpStartStatsResult
    {
        public int UserCount { get; }

        public int UserGrowthPercent { get; }

        public int OpenCount { get; }

        public int OpenGrowthPercent { get; }

        public int ClickCount { get; }

        public int ClickGrowthPercent { get; }

        public List<JumpStartDailyStatsResult> Users { get; }

        public List<JumpStartDailyStatsResult> Opens { get; }

        public List<JumpStartDailyStatsResult> Clicks { get; }

        public List<JumpStartDailyTagStatsResult> Tags { get; }

        //public List<JumpStartHeatMapResult> HeatMap { get; set; } = new List<JumpStartHeatMapResult>();

        public JumpStartStatsResult(JumpStartStats jumpStartStats)
        {
            Users = jumpStartStats.Users.Select(item => new JumpStartDailyStatsResult(item)).ToList();
            Opens = jumpStartStats.Opens.Select(item => new JumpStartDailyStatsResult(item)).ToList();
            Clicks = jumpStartStats.Clicks.Select(item => new JumpStartDailyStatsResult(item)).ToList();
            Tags = jumpStartStats.Tags.Select(item => new JumpStartDailyTagStatsResult(item)).ToList();
            
            UserCount = Users.LastOrDefault()?.Count ?? 0;
            UserGrowthPercent =  (Users.LastOrDefault()?.Count ?? 0 / Users.FirstOrDefault()?.Count ?? 1) * 100;

            OpenCount = Opens.LastOrDefault()?.Count ?? 0;
            OpenGrowthPercent = (Opens.LastOrDefault()?.Count ?? 0 / Opens.FirstOrDefault()?.Count ?? 1) * 100;

            ClickCount = Clicks.LastOrDefault()?.Count ?? 0;
            ClickGrowthPercent = (Clicks.LastOrDefault()?.Count ?? 0 / Clicks.FirstOrDefault()?.Count ?? 1) * 100;
        }
    }

    public class JumpStartDailyStatsResult
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; }

        public int Count { get; }

        public JumpStartDailyStatsResult(JumpStartDailyStats jumpStartDailyStats)
        {
            Date = jumpStartDailyStats.Date;
            Count = jumpStartDailyStats.Count;
        }
    }

    public class JumpStartDailyTagStatsResult
    {
        public string Tag { get; }

        public List<JumpStartDailyStatsResult> Stats { get; }

        public JumpStartDailyTagStatsResult(JumpStartDailyTagStats jumpStartDailyTagStats)
        {
            Tag = jumpStartDailyTagStats.Tag;
            Stats = jumpStartDailyTagStats.Stats.Select(item => new JumpStartDailyStatsResult(item)).ToList();
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