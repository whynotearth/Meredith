using System;

namespace WhyNotEarth.Meredith.Volkswagen
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

        public DateTime CreatedAt { get; set; }
    }
}