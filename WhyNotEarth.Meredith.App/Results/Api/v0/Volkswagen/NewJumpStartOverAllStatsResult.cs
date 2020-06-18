using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class NewJumpStartOverAllStatsResult : OverAllStatsResult
    {
        public List<JumpStartDailyTagStatsResult> Tags { get; }

        //public List<JumpStartHeatMapResult> HeatMap { get; set; } = new List<JumpStartHeatMapResult>();

        public NewJumpStartOverAllStatsResult(NewJumpStartOverAllStats newJumpStartOverAllStats) : base(newJumpStartOverAllStats)
        {
            Tags = newJumpStartOverAllStats.Tags.Select(item => new JumpStartDailyTagStatsResult(item)).ToList();
        }
    }

    public class JumpStartDailyTagStatsResult
    {
        public string Tag { get; }

        public List<DailyStatsResult> Stats { get; }

        public JumpStartDailyTagStatsResult(NewJumpStartDailyTagStats newJumpStartDailyTagStats)
        {
            Tag = newJumpStartDailyTagStats.Tag;
            Stats = newJumpStartDailyTagStats.Stats.Select(item => new DailyStatsResult(item)).ToList();
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