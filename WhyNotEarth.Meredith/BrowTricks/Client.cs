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

        public List<ClientNote>? Notes { get; set; }

        public List<ClientImage>? Images { get; set; }

        public List<ClientVideo>? Videos { get; set; }

        public bool IsArchived { get; set; }
    }

    public class ClientImage : Image
    {
        public int? ClientId { get; set; }

        public Client? Client { get; set; }
    }

    public class ClientVideo : Video
    {
        public int? ClientId { get; set; }

        public Client? Client { get; set; }
    }
}