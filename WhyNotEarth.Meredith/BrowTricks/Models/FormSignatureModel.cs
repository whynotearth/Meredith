using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.BrowTricks.Models
{
    public class FormSignatureModel
    {
        [NotNull]
        [Mandatory]
        public List<FormSignatureItemModel>? Answers { get; set; }

        public string? NotificationCallBackUrl { get; set; }
    }

    public class FormSignatureItemModel
    {
        [NotNull]
        [Mandatory]
        public int? FormItemId { get; set; }

        [NotNull]
        [Mandatory]
        public List<string>? Value { get; set; }
    }
}