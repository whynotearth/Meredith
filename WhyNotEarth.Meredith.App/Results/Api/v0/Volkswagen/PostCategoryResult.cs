using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class PostCategoryResult
    {
        public int Id { get; }

        public string Slug { get; }

        public string Name { get; }

        public string Color { get; }

        public string? Image { get; }

        public string Description { get; }

        public PostCategoryResult(PostCategory category)
        {
            Id = category.Id;
            Slug = category.Slug;
            Name = category.Name;
            Color = category.Color;
            Image = category.Image?.Url;
            Description = category.Description;
        }
    }
}