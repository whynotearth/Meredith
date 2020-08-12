using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Persistence.Models
{
    public class Video
    {
        public int Id { get; set; }

        public string CloudinaryPublicId { get; set; } = null!;

        public string Url { get; set; } = null!;
    }

    public class VideoEntityConfig : IEntityTypeConfiguration<Video>
    {
        public void Configure(EntityTypeBuilder<Video> builder)
        {
            builder.ToTable("Videos", "public");
        }
    }
}