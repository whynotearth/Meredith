using System;
using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen.Post
{
    public class PostResult
    {
        public int Id { get; }

        public string Headline { get; }

        public string Description { get; }

        public DateTime Date { get; }

        public List<ImageResult> Images { get; set; } = new List<ImageResult>();

        public PostResult(int id, string headline, string description, DateTime date)
        {
            Id = id;
            Headline = headline;
            Description = description;
            Date = date;
        }

        public PostResult(int id, string headline, string description, DateTime date, List<PostImage> images) : this(id,
            headline, description, date)
        {
            if (images is null)
            {
                return;
            }

            foreach (var postImage in images)
            {
                Images.Add(new ImageResult(postImage.Order, postImage.Url));
            }
        }
    }
}