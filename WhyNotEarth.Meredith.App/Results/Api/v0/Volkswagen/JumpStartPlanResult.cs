using System;
using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class JumpStartPlanResult
    {
        public int? JumpStartId { get; }

        public DateTime DateTime { get; }

        public List<string> DistributionGroups { get; }

        public List<ArticleResult> Articles { get; }

        public JumpStartPlanResult(JumpStartPlan jumpStartPlan)
        {
            JumpStartId = jumpStartPlan.JumpStart?.Id;
            DateTime = jumpStartPlan.DateTime;
            DistributionGroups = jumpStartPlan.DistributionGroups;
            Articles = jumpStartPlan.Articles.Select(item => new ArticleResult(item)).ToList();
        }
    }
}