namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Card : IEntityTypeConfiguration<Card>
    {
        public string BackgroundUrl { get; set; }

        public string CallToAction { get; set; }

        public string CallToActionUrl { get; set; }

        public Guid Id { get; set; }

        public Page Page { get; set; }
        
        public Guid PageId { get; set; }
        
        public string PosterUrl { get; set; }

        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.Property(e => e.BackgroundUrl).HasMaxLength(256);
            builder.Property(e => e.CallToAction).HasMaxLength(256);
            builder.Property(e => e.CallToActionUrl).HasMaxLength(256);
            builder.Property(e => e.PosterUrl).HasMaxLength(256);
        }
    }
}