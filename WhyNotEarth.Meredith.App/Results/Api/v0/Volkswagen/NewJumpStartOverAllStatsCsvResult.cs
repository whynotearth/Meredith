using System;
using System.ComponentModel.DataAnnotations;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class NewJumpStartOverAllStatsCsvResult
    {
        public NewJumpStartOverAllStatsTypeCsvResult Type { get; }

        [DataType(DataType.Date)]
        public DateTime Date { get; }

        public int Count { get; }

        public string? Tag { get; set; }

        public NewJumpStartOverAllStatsCsvResult(NewJumpStartOverAllStatsTypeCsvResult type, DailyStats dailyStats)
        {
            Type = type;
            Date = dailyStats.Date;
            Count = dailyStats.Count;
        }

        public NewJumpStartOverAllStatsCsvResult(NewJumpStartOverAllStatsTypeCsvResult type, DailyStats dailyStats, string tag)
        {
            Type = type;
            Date = dailyStats.Date;
            Count = dailyStats.Count;
            Tag = tag;
        }
    }

    public enum NewJumpStartOverAllStatsTypeCsvResult
    {
        User,
        Open,
        Click,
        Tag
    }
}