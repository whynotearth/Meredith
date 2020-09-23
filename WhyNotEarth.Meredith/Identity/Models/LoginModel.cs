using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Identity.Models
{
    public class LoginModel
    {
        [NotNull]
        [Mandatory]
        public string? Email { get; set; }

        [NotNull]
        [Mandatory]
        public string? Password { get; set; }
    }
}