using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Authentication
{
    public class ForgotPasswordModel
    {
        [Required]
        public string CompanySlug { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string ReturnUrl { get; set; } = null!;
    }
}