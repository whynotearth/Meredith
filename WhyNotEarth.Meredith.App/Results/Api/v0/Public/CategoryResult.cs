using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public
{
    public class CategoryResult
    {
        public int Id { get; set; }

        public string? Slug { get; set; }

        public string? Name { get; set; }

        public string? Image { get; set; }

        public string? Description { get; set; }

        public CategoryResult(Category category)
        {
            Id = category.Id;
            Slug = category.Slug;
            Name = category.Name;
            Image = category.Image?.Url;
            Description = category.Description;
        }
    }
}