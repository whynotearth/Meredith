using System.ComponentModel.DataAnnotations;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Volkswagen.Models
{
    public class RecipientModel
    {
        [Mandatory]
        [EmailAddress]
        public string? Email { get; set; }

        public string? FirstName { get; set; }
        
        public string? LastName { get; set; }
    }
}