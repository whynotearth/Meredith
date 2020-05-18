using System;
using System.Collections.Generic;
using WhyNotEarth.Meredith.App.Validation;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen
{
    public class JumpStartModel
    {
        public int? Id { get; set; }

        [Mandatory]
        public DateTime? DateTime { get; set; }

        [Mandatory]
        public List<string>? DistributionGroups { get; set; }

        [Mandatory]
        public List<int>? ArticleIds { get; set; }
    }
}