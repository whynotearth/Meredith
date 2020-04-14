using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class Keyword : IEntityTypeConfiguration<Keyword>
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public int PageId { get; set; }

        public Page Page { get; set; }

        public void Configure(EntityTypeBuilder<Keyword> builder)
        {
        }
    }
}
