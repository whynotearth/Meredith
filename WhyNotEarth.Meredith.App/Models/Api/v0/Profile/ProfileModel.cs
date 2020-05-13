using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Profile
{
    public class ProfileModel
    {
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;
    }
}