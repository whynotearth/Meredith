using System;
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

        public PostImage Image { get; set; }

        public PostCategory Category { get; set; }

        public JumpStart JumpStart { get; set; }

        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts", "ModuleVolkswagen");
            builder.Property(e => e.Date).HasColumnType("date");
            builder.Property(e => e.EventDate).HasColumnType("date");
        }
    }

    public class PostImage : Image
    {
    }

    public class PostCategory : Category, IEntityTypeConfiguration<PostCategory>
    {
        public string Color { get; set; }

        public int Priority { get; set; }

        public void Configure(EntityTypeBuilder<PostCategory> builder)
        {
            builder.Property(b => b.Color).IsRequired();
        }
    }
}