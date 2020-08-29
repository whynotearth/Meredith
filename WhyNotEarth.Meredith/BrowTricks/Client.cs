using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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

        public NotificationType NotificationType { get; set; }

        public List<ClientNote>? Notes { get; set; }

        public List<ClientImage>? Images { get; set; }

        public List<ClientVideo>? Videos { get; set; }

        public PmuStatusType PmuStatus { get; set; }

        public bool IsArchived { get; set; }

        public string? PmuPdf { get; set; }

        public DateTime? SignedAt { get; set; }

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

    public enum PmuStatusType
    {
        [EnumMember(Value = "incomplete")]
        Incomplete = 1,

        [EnumMember(Value = "saving")]
        Saving = 2,

        [EnumMember(Value = "completed")]
        Completed = 3
    }
}