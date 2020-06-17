using System;
using System.ComponentModel.DataAnnotations;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class OverAllStatsCsvResult
    {
        public OverAllStatsTypeCsvResult Type { get; }

        [DataType(DataType.Date)]
        public DateTime Date { get; }

        public int Count { get; }

        public OverAllStatsCsvResult(OverAllStatsTypeCsvResult type, DailyStats dailyStats)
        {
            Type = type;
            Date = dailyStats.Date;
            Count = dailyStats.Count;
        }
    }

    public enum OverAllStatsTypeCsvResult
    {
        User,
        Open,
        Click
    }
}