using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    [DebuggerDisplay("{" + nameof(Slug) + "}")]
    public class Company : IEntityTypeConfiguration<Company>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Page> Pages { get; set; }

        public ICollection<Tenant> Tenants { get; set; }

        public string Slug { get; set; }

        public void Configure(EntityTypeBuilder<Company> builder)
        {
        }
    }
}