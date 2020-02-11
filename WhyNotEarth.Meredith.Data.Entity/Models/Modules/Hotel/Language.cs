using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    public class Language : IEntityTypeConfiguration<Language>
    {
        public int Id { get; set; }

        public string Culture { get; set; }

        public string Name { get; set; }

        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("Languages", "ModuleHotel");
        }
    }
}