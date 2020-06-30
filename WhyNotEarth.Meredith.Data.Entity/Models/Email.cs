#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class Email
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        public int? MemoId { get; set; }

        public Memo? Memo { get; set; }

        public int? JumpStartId { get; set; }

        public JumpStart? JumpStart { get; set; }

        public int? NewJumpStartId { get; set; }

        public NewJumpStart? NewJumpStart { get; set; }

        public string EmailAddress { get; set; } = null!;

        public string? Group { get; set; }

        public EmailStatus Status { get; set; }

        public DateTime CreationDateTime { get; set; }

        public List<EmailEvent>? Events { get; set; }
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

    public class EmailEntityConfig : IEntityTypeConfiguration<Email>
    {
        public void Configure(EntityTypeBuilder<Email> builder)
        {
            builder.ToTable("Emails", "public");
        }
    }
}