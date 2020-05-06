using System;
using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen
{
    public class JumpStartPreviewResult
    {
        public DateTime Date { get; }

        public List<PostResult> Posts { get; }

        public JumpStartPreviewResult(DateTime date, List<Post>? posts)
        {
            Date = date;
            Posts = posts.Select(item => new PostResult(item)).ToList() ?? new List<PostResult>();
        }
    }

    public class PostResult
    {
        public int Id { get; }

        public PostCategoryResult Category { get; }

        public string Headline { get; }

        public string Description { get; }

        public DateTime Date { get; }

        public decimal? Price { get; }

        public DateTime? EventDate { get; }

        public ImageResult? Image { get; }

        public PostResult(Post post)
        {
            Id = post.Id;
            Category = new PostCategoryResult(post.Category);
            Headline = post.Headline;
            Description = post.Description;
            Date = post.Date;
            Price = post.Price;
            EventDate = post.EventDate;

            if (post.Image != null)
            {
                Image = new ImageResult(post.Image);
            }
        }
    }
}