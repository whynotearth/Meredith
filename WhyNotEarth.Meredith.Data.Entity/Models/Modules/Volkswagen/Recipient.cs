using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen
{
    public class Recipient : IEntityTypeConfiguration<Recipient>
    {
        public static string ModuleName { get; } = "ModuleVolkswagen";

        public static string TableName { get; } = "Recipients";

        public int Id { get; set; }

        public string Email { get; set; }
        
        public string? FirstName { get; set; }
        
        public string? LastName { get; set; }

        public string DistributionGroup { get; set; }

        public DateTime CreationDateTime { get; set; }

        public void Configure(EntityTypeBuilder<Recipient> builder)
        {
            builder.ToTable(TableName, ModuleName);
        }
    }
}