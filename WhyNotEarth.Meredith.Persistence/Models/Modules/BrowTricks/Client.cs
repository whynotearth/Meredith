using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.BrowTricks;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.BrowTricks
{
    public class ClientEntityConfig : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients", "ModuleBrowTricks");

            builder.HasMany(e => e.Images)
                .WithOne(i => i.Client!)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Videos)
                .WithOne(i => i.Client!)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}