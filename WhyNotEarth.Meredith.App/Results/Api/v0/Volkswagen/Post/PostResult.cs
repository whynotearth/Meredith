using System;
using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen.Post
{
    public class PostResult
    {
        public int Id { get; }

        public string Headline { get; }

        public string Description { get; }

        public DateTime Date { get; }

        public decimal Price { get; }

        public DateTime EventDate { get; }

        public List<ImageResult> Images { get; }

        public PostResult(Data.Entity.Models.Modules.Volkswagen.Post post)
        {
            Id = post.Id;
            Headline = post.Headline;
            Description = post.Description;
            Date = post.Date;
            Price = post.Price;
            EventDate = post.EventDate;

            Images = post.Images != null
                ? post.Images.Select(item => new ImageResult(item)).ToList()
                : new List<ImageResult>();
        }
    }
}