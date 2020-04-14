using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen
{
    public class Recipient : IEntityTypeConfiguration<Recipient>
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public void Configure(EntityTypeBuilder<Recipient> builder)
        {
            builder.ToTable("Recipients", "ModuleVolkswagen");
        }
    }
}