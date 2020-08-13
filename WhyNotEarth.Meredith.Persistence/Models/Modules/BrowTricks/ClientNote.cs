using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.BrowTricks;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.BrowTricks
{
    public class ClientNoteEntityConfig : IEntityTypeConfiguration<ClientNote>
    {
        public void Configure(EntityTypeBuilder<ClientNote> builder)
        {
            builder.ToTable("ClientNotes", "ModuleBrowTricks");
        }
    }
}