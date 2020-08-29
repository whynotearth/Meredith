using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.BrowTricks.Models
{
    public class DisclosureModel
    {
        [NotNull]
        [Mandatory]
        public List<DisclosureItemModel>? Disclosures { get; set; }
    }

    public class DisclosureItemModel
    {
        public int? Id { get; set; }

        [NotNull]
        [Mandatory]
        public string? Value { get; set; }
    }
}