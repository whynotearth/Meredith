namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Page : IEntityTypeConfiguration<Page>
    {
        public Company Company { get; set; }
        
        public Guid CompanyId { get; set; }
        
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Slug { get; set; }
        
        public void Configure(EntityTypeBuilder<Page> builder)
        {
            throw new System.NotImplementedException();
        }
    }
}