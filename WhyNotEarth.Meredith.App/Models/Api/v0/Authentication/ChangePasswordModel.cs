using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Authentication
{
    public class ChangePasswordModel
    {

        public string? OldPassword { get; set; } = null!;

        [Required]
        public string NewPassword { get; set; } = null!;

        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; } = null!;
    }
}