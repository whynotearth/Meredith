using System.Collections.Generic;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public class Client
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public Public.Tenant Tenant { get; set; } = null!;

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string FullName => string.Join(' ', FirstName, LastName);

        public string? PhoneNumber { get; set; }

        public string? AvatarUrl { get; set; }

        public List<ClientNote>? Notes { get; set; }

        public List<BrowTricksImage> Images { get; set; } = new List<BrowTricksImage>();

        public List<BrowTricksVideo> Videos { get; set; } = new List<BrowTricksVideo>();

        public bool IsArchived { get; set; }
    }

    public class BrowTricksImage : Image
    {
        public int TenantId { get; set; }

        public Public.Tenant? Tenant { get; set; }

        public int? ClientId { get; set; }

        public Client? Client { get; set; }
    }

    public class BrowTricksVideo : Video
    {
        public int TenantId { get; set; }

        public Public.Tenant? Tenant { get; set; }

        public int? ClientId { get; set; }

        public Client? Client { get; set; }
    }
}