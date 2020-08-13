using System;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartPlan
    {
        public DateTime DateTime { get; }

        public List<Article> Articles { get; }

        public List<string> DistributionGroups { get; }

        public JumpStart? JumpStart { get; }

        public JumpStartPlan(DateTime dateTime, List<Article> articles, List<string> distributionGroups,
            JumpStart? jumpStart)
        {
            DateTime = dateTime;
            Articles = articles;
            DistributionGroups = distributionGroups;
            JumpStart = jumpStart;
        }
    }
}