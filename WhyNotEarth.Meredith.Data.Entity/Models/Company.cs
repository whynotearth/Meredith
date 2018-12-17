namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Company : IEntityTypeConfiguration<Company>
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Slug { get; set; }
        
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.Property(e => e.Name).HasMaxLength(64);
            builder.Property(e => e.Slug).HasMaxLength(64);
        }
    }
}