using System;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Twilio
{
    public class ShortMessage
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        public int? TenantId { get; set; }

        public Public.Tenant? Tenant { get; set; }

        public string To { get; set; } = null!;

        public string Body { get; set; } = null!;

        public bool IsWhatsApp { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? SentAt { get; set; }
    }
}