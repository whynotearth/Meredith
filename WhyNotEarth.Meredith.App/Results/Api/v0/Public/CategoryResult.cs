using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public
{
    public class CategoryResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Image { get; set; }

        public string Description { get; set; }

        public CategoryResult(Category category)
        {
            Id = category.Id;
            Name = category.Name;
            Image = category.Image?.Url;
            Description = category.Description;
        }
    }
}