using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.BrowTricks.Models
{
    public class PmuSignModel
    {
        [NotNull]
        [Mandatory]
        public List<FormAnswerModel>? Answers { get; set; }
    }

    public class FormAnswerModel
    {
        [NotNull]
        [Mandatory]
        public int? FormItemId { get; set; }

        [NotNull]
        [Mandatory]
        public List<string>? Value { get; set; }
    }
}