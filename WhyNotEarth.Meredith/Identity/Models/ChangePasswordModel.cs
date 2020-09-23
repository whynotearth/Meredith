using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Identity.Models
{
    public class ChangePasswordModel
    {
        public string? OldPassword { get; set; }

        [NotNull]
        [Mandatory]
        public string? NewPassword { get; set; }

        [Compare(nameof(NewPassword))]
        public string? ConfirmPassword { get; set; }
    }
}