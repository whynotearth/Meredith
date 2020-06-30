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
}