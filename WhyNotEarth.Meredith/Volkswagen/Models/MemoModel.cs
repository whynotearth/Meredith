using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Volkswagen.Models
{
    public class MemoModel
    {
        [NotNull]
        [Mandatory]
        public List<string>? DistributionGroups { get; set; }

        [NotNull]
        [Mandatory]
        public string? Subject { get; set; }

        [NotNull]
        [Mandatory]
        public string? Date { get; set; }

        [NotNull]
        [Mandatory]
        public string? To { get; set; }

        [NotNull]
        [Mandatory]
        public string? Description { get; set; }

        public string? PdfUrl { get; set; }
    }
}