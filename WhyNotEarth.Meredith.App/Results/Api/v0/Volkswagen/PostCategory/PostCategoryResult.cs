namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen.PostCategory
{
    public class PostCategoryResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public PostCategoryResult(Data.Entity.Models.Category category)
        {
            Id = category.Id;
            Name = category.Name;
            Image = category.Image?.Url;
            Description = category.Description;
        }
    }
}