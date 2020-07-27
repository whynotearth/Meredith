using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.BrowTricks.Models
{
    public class PmuQuestionCreateModel
    {
        [NotNull]
        [Mandatory]
        public List<string> Questions { get; set; } = null!;
    }
}