﻿using System;

namespace WhyNotEarth.Meredith.Public
{
    public class ShortMessage
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        public int? TenantId { get; set; }

        public Tenant? Tenant { get; set; }

        public string To { get; set; } = null!;

        public string Body { get; set; } = null!;

        public bool IsWhatsApp { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? SentAt { get; set; }
    }
}