using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class EmailEvent : IEntityTypeConfiguration<EmailEvent>
    {
        public int Id { get; set; }

        public int EmailId { get; set; }

        public Email Email { get; set; }

        public EmailEventType Type { get; set; }

        public DateTime DateTime { get; set; }

        public void Configure(EntityTypeBuilder<EmailEvent> builder)
        {
            builder.ToTable("EmailEvents", "public");
        }
    }

    public enum EmailEventType : byte
    {
        None = 0,
        Delivered = 1,
        Opened = 2,
        Clicked = 3
    }
}