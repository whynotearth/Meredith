using System.ComponentModel.DataAnnotations;
using WhyNotEarth.Meredith.App.Validation;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen
{
    public class RecipientModel
    {
        [Mandatory]
        [EmailAddress]
        public string? Email { get; set; }
    }
}