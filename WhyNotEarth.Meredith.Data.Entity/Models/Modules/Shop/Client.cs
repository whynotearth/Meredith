using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public class Client : IEntityTypeConfiguration<Client>
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public Tenant Tenant { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients", "ModuleShop");
        }
    }
}