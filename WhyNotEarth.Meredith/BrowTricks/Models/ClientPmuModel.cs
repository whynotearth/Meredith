using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.BrowTricks.Models
{
    public class ClientPmuModel
    {
        [NotNull]
        [Mandatory]
        public string? Signature { get; set; }

        [NotNull]
        [Mandatory]
        public string? Initials { get; set; }

        [NotNull]
        [Mandatory]
        public bool? AllowPhoto { get; set; }

        [NotNull]
        [Mandatory]
        public bool? IsUnderCareOfPhysician { get; set; }

        [NotNull]
        [Mandatory]
        public string? Conditions { get; set; }

        [NotNull]
        [Mandatory]
        public bool? IsTakingBloodThinner { get; set; }

        [NotNull]
        [Mandatory]
        public string? PhysicianName { get; set; }

        [NotNull]
        [Mandatory]
        public string? PhysicianPhoneNumber { get; set; }

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