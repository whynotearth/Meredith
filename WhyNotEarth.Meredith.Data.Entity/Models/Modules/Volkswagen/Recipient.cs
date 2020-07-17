using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen
{
    public class Recipient
    {
        public static string ModuleName { get; } = "ModuleVolkswagen";

        public static string TableName { get; } = "Recipients";

        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string DistributionGroup { get; set; } = null!;

        public DateTime CreationDateTime { get; set; }
    }

    public class RecipientEntityConfig : IEntityTypeConfiguration<Recipient>
    {
        public void Configure(EntityTypeBuilder<Recipient> builder)
        {
            builder.ToTable(Recipient.TableName, Recipient.ModuleName);
        }
    }
}