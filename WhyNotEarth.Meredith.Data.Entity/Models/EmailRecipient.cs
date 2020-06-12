using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class EmailRecipient : IEntityTypeConfiguration<EmailRecipient>
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; }

        public int? MemoId { get; set; }

        public Memo Memo { get; set; }

        public int? JumpStartId { get; set; }

        public JumpStart JumpStart { get; set; }

        public int? NewJumpStartId { get; set; }

        public NewJumpStart NewJumpStart { get; set; }

        public string Email { get; set; }

        public string DistributionGroup { get; set; }

        public EmailStatus Status { get; set; }

        public DateTime CreationDateTime { get; set; }

        public DateTime? DeliverDateTime { get; set; }

        public DateTime? OpenDateTime { get; set; }

        public DateTime? ClickDateTime { get; set; }

        public void Configure(EntityTypeBuilder<EmailRecipient> builder)
        {
            builder.ToTable("EmailRecipients", "public");
        }
    }

    public enum EmailStatus : byte
    {
        None = 0,
        ReadyToSend = 1,
        Sent = 2,
        Delivered = 3,
        Opened = 4,
        Clicked = 5
    }
}