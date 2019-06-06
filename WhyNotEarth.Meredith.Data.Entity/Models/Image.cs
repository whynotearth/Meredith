namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Image : IEntityTypeConfiguration<Image>
    {
        public int Id { get; set; }

        public int Order { get; set; }

        public Page Page { get; set; }

        public int PageId { get; set; }

        public string Url { get; set; }

        public void Configure(EntityTypeBuilder<Image> builder)
        {

        }
    }
}