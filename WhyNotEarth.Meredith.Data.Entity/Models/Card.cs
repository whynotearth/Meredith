using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class Card : IEntityTypeConfiguration<Card>
    {
        public enum CardTypes
        {
            Card
        }

        public string BackgroundUrl { get; set; }

        public string CallToAction { get; set; }

        public string CallToActionUrl { get; set; }

        public int Id { get; set; }

        public Page Page { get; set; }

        public int PageId { get; set; }

        public string PosterUrl { get; set; }

        public CardTypes CardType { get; set; }

        public int Order { get; set; }

        public string Text { get; set; }

        public void Configure(EntityTypeBuilder<Card> builder)
        {
        }
    }
}