using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Identity.Models
{
    public class TokenLoginModel
    {
        [NotNull]
        [Mandatory]
        public string? Token { get; set; }
    }
}