using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models
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

    public class ShortMessageConfig : IEntityTypeConfiguration<ShortMessage>
    {
        public void Configure(EntityTypeBuilder<ShortMessage> builder)
        {
            builder.ToTable("ShortMessages", "public");
        }
    }
}