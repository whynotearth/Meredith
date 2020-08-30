using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Authentication
{
    public class TokenLoginModel
    {
        [NotNull]
        [Mandatory]
        public string Token { get; set; } = null!;
    }
}