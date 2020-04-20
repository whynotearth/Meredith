using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class Category : IEntityTypeConfiguration<Category>
    {
        public int Id { get; set; }
        
        public string Slug { get; set; }

        public string Name { get; set; }

        public CategoryImage Image { get; set; }

        public string Description { get; set; }

        public void Configure(EntityTypeBuilder<Category> builder)
        {
        }
    }

    public class CategoryImage : Image
    {
    }
}