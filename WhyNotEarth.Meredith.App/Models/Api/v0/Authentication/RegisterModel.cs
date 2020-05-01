using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Authentication
{
    public class RegisterModel
    {
        [Required]
        public string Email { get; set; } = null!;

        public string? Password { get; set; }

        public string? Name { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? GoogleLocation { get; set; }
    }
}