using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.BrowTricks.Models
{
    public class ClientPmuModel
    {
        public List<PmuAnswerModel>? Answers { get; set; }
    }

    public class PmuAnswerModel
    {
        [NotNull]
        [Mandatory]
        public int? QuestionId { get; set; }

        public string? Answer { get; set; }
    }
}