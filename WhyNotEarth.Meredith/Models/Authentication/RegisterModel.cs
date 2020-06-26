using Hangfire.Annotations;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Models
{
    public class RegisterModel
    {
        [NotNull]
        [Mandatory]
        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? FirstName { get; set; }
        
        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? GoogleLocation { get; set; }

        public string? TenantSlug { get; set; }
    }
}