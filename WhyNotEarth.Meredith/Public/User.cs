using Microsoft.AspNetCore.Identity;

namespace WhyNotEarth.Meredith.Public
{
    public class User : IdentityUser<int>
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string FullName => string.Join(' ', FirstName, LastName);

        public string? ImageUrl { get; set; }

        public string? Address { get; set; }

        public string? GoogleLocation { get; set; }

        public bool WhatsappNotification { get; set; }

        public string? FacebookUrl { get; set; }

        public string? InstagramUrl { get; set; }

        public int? TenantId { get; set; }

        public Tenant? Tenant { get; set; }

        public string GetDisplayName()
        {
            if (FirstName != null || LastName != null)
            {
                return FullName;
            }

            if (UserName != null)
            {
                return UserName;
            }

            return Email;
        }
    }
}