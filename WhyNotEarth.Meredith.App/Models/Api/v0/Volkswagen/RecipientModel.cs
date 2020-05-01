using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen
{
    public class RecipientModel
    {
        [Required]
        public string Email { get; set; } = null!;
    }
}