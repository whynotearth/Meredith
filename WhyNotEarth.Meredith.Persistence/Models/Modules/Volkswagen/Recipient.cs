using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Volkswagen
{
    public class RecipientEntityConfig : IEntityTypeConfiguration<Recipient>
    {
        public void Configure(EntityTypeBuilder<Recipient> builder)
        {
            builder.ToTable(Recipient.TableName, Recipient.ModuleName);
        }
    }
}