using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class OverAllStatsCsvResult
    {
        public OverAllStatsTypeCsvResult Type { get; }

        [DataType(DataType.Date)]
        public DateTime Date { get; }

        public int Count { get; }

        private OverAllStatsCsvResult(OverAllStatsTypeCsvResult type, DailyStats dailyStats)
        {
            Type = type;
            Date = dailyStats.Date;
            Count = dailyStats.Count;
        }

        public static List<OverAllStatsCsvResult> Create(OverAllStats stats)
        {
            var result = new List<OverAllStatsCsvResult>();

            result.AddRange(stats.Users.Select(item =>
                new OverAllStatsCsvResult(OverAllStatsTypeCsvResult.User, item)));

            result.AddRange(stats.Opens.Select(item =>
                new OverAllStatsCsvResult(OverAllStatsTypeCsvResult.Open, item)));

            result.AddRange(
                stats.Clicks.Select(item => new OverAllStatsCsvResult(OverAllStatsTypeCsvResult.Click, item)));

            return result;
        }
    }

    public enum OverAllStatsTypeCsvResult
    {
        User,
        Open,
        Click
    }
}