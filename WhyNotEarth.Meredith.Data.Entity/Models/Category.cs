namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Category : IEntityTypeConfiguration<Category>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public CategoryImage Image { get; set; }

        public string Description { get; set; }

        public void Configure(EntityTypeBuilder<Category> builder)
        {
        }
    }
}