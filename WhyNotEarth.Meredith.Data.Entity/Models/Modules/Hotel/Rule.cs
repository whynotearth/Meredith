namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Rule : IEntityTypeConfiguration<Rule>
    {
        public Hotel Hotel { get; set; }

        public int HotelId { get; set; }

        public int Id { get; set; }

        public ICollection<RuleTranslation> Translations { get; set; }

        public void Configure(EntityTypeBuilder<Rule> builder)
        {
            builder.ToTable("Rules", "ModuleHotel");
        }
    }
}