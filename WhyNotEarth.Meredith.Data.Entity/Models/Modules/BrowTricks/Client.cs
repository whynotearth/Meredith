using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks
{
    public class Client
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public NotificationType NotificationType { get; set; }

        public List<ClientNote>? Notes { get; set; }

        public List<ClientImage>? Images { get; set; }

        public List<ClientVideo>? Videos { get; set; }

        public bool IsPmuCompleted { get; set; }

        public string? Signature { get; set; }

        public string? Initials { get; set; }

        public bool? AllowPhoto { get; set; }

        public bool? IsUnderCareOfPhysician { get; set; }

        public string? Conditions { get; set; }

        public bool? IsTakingBloodThinner { get; set; }

        public string? PhysicianName { get; set; }

        public string? PhysicianPhoneNumber { get; set; }

        public bool IsArchived { get; set; }

        public List<PmuAnswer>? PmuAnswers { get; set; }

        public string? SignatureRequestId { get; set; }

        public string? PmuPdf { get; set; }
    }

    public class ClientImage : Image
    {
        public int? ClientId { get; set; }

        public Client? Client { get; set; }
    }

    public class ClientVideo : Video
    {
        public int? ClientId { get; set; }

        public Client? Client { get; set; }
    }

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