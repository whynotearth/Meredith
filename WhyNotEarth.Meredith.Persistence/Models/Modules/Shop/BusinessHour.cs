using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Shop;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Shop
{
    public class BusinessHourEntityConfig : IEntityTypeConfiguration<BusinessHour>
    {
        public void Configure(EntityTypeBuilder<BusinessHour> builder)
        {
            builder.ToTable("BusinessHours", "ModuleShop");
        }
    }
}