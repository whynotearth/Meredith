using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class ArticleCategoryResult
    {
        public string Slug { get; }

        public string Name { get; }

        public string Color { get; }

        public string? Image { get; }

        public string Description { get; }

        public ArticleCategoryResult(ArticleCategory category)
        {
            Slug = category.Slug;
            Name = category.Name;
            Color = category.Color;
            Image = category.Image?.Url;
            Description = category.Description;
        }
    }
}