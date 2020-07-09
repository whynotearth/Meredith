using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

#nullable enable

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string? Slug { get; set; }

        public string Name { get; set; } = null!;

        public CategoryImage? Image { get; set; }

        public string? Description { get; set; }
    }

    public class CategoryImage : Image
    {
    }

    public class CategoryEntityConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
        }
    }
}