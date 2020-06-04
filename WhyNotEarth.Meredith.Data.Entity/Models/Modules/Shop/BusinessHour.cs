using System;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public class BusinessHour : IEntityTypeConfiguration<BusinessHour>
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public Tenant Tenant { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public bool IsClosed { get; set; }

        public TimeSpan? OpeningTime { get; set; }

        public TimeSpan? ClosingTime { get; set; }

        public void Configure(EntityTypeBuilder<BusinessHour> builder)
        {
        }
    }

    public enum DayOfWeek : byte
    {
        [EnumMember(Value = "sunday")]
        Sunday = 1,

        [EnumMember(Value = "monday")]
        Monday = 2,

        [EnumMember(Value = "tuesday")]
        Tuesday = 3,

        [EnumMember(Value = "wednesday")]
        Wednesday = 4,

        [EnumMember(Value = "thursday")]
        Thursday = 5,

        [EnumMember(Value = "friday")]
        Friday = 6,
        
        [EnumMember(Value = "saturday")]
        Saturday = 7
    }
}