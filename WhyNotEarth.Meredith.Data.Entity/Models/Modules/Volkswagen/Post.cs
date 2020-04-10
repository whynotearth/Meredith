using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen
{
    public class Post : IEntityTypeConfiguration<Post>
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string Headline  { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public decimal? Price { get; set; }

        public DateTime? EventDate { get; set; }

        public int? JumpStartId { get; set; }

        public int? Order { get; set; }

        public List<PostImage> Images { get; set; }

        public PostCategory Category { get; set; }

        public JumpStart JumpStart { get; set; }

        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts", "ModuleVolkswagen");
        }
    }

    public class PostImage : Image
    {
    }

    public class PostCategory : Category
    {
        public int Priority { get; set; }
    }
}