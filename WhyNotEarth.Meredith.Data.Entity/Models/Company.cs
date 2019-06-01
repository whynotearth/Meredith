namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Company : IEntityTypeConfiguration<Company>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Page> Pages { get; set; }

        public string Slug { get; set; }

        public void Configure(EntityTypeBuilder<Company> builder)
        {
        }
    }
}