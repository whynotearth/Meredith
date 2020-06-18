using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        private NewJumpStartOverAllStatsCsvResult(NewJumpStartOverAllStatsTypeCsvResult type, DailyStats dailyStats)
        {
            Type = type;
            Date = dailyStats.Date;
            Count = dailyStats.Count;
        }

        private NewJumpStartOverAllStatsCsvResult(NewJumpStartOverAllStatsTypeCsvResult type, DailyStats dailyStats,
            string tag)
        {
            Type = type;
            Date = dailyStats.Date;
            Count = dailyStats.Count;
            Tag = tag;
        }

        public static List<NewJumpStartOverAllStatsCsvResult> Create(NewJumpStartOverAllStats stats)
        {
            var result = new List<NewJumpStartOverAllStatsCsvResult>();

            result.AddRange(stats.Users.Select(item =>
                new NewJumpStartOverAllStatsCsvResult(NewJumpStartOverAllStatsTypeCsvResult.User, item)));

            result.AddRange(stats.Opens.Select(item =>
                new NewJumpStartOverAllStatsCsvResult(NewJumpStartOverAllStatsTypeCsvResult.Open, item)));

            result.AddRange(stats.Clicks.Select(item =>
                new NewJumpStartOverAllStatsCsvResult(NewJumpStartOverAllStatsTypeCsvResult.Click, item)));

            foreach (var tagStat in stats.Tags)
            {
                result.AddRange(tagStat.Stats.Select(item =>
                    new NewJumpStartOverAllStatsCsvResult(NewJumpStartOverAllStatsTypeCsvResult.Tag, item,
                        tagStat.Tag)));
            }

            return result;
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