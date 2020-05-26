using System.Collections.Generic;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen
{
    public class MemoModel
    {
        [Mandatory]
        public List<string>? DistributionGroups { get; set; }

        [Mandatory]
        public string? Subject { get; set; }

        [Mandatory]
        public string? Date { get; set; }

        [Mandatory]
        public string? To { get; set; }

        [Mandatory]
        public string? Description { get; set; }
    }
}