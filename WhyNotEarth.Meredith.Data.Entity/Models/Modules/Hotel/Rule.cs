using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    public class Rule
    {
        public int Id { get; set; }

        public int HotelId { get; set; }

        public Hotel Hotel { get; set; } = null!;

        public ICollection<RuleTranslation>? Translations { get; set; }
    }

    public class RuleEntityConfig : IEntityTypeConfiguration<Rule>
    {
        public void Configure(EntityTypeBuilder<Rule> builder)
        {
            builder.ToTable("Rules", "ModuleHotel");
        }
    }
}