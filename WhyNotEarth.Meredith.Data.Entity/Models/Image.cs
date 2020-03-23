using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class Image : IEntityTypeConfiguration<Image>
    {
        public int Id { get; set; }

        public int Order { get; set; }

        public string Url { get; set; }

        public void Configure(EntityTypeBuilder<Image> builder)
        {
        }
    }
}