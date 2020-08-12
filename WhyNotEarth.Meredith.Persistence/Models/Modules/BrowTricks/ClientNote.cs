using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.BrowTricks
{
    public class ClientNote
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; } = null!;

        public string Note { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }

    public class ClientNoteEntityConfig : IEntityTypeConfiguration<ClientNote>
    {
        public void Configure(EntityTypeBuilder<ClientNote> builder)
        {
            builder.ToTable("ClientNotes", "ModuleBrowTricks");
        }
    }
}