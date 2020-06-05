namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.Page
{
    public class PageCategoryResult
    {
        public int Id { get;  }

        public string? Slug { get;  }

        public string? Name { get;  }

        public string? Image { get;  }

        public string? Description { get; }
        public PageCategoryResult(int id, string slug, string name, string image, string description)
        {
            this.Id = id;
            this.Slug = slug;
            this.Name = name;
            this.Image = image;
            this.Description = description;
        }
    }
}
