namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Hotel : IEntityTypeConfiguration<Hotel>
    {
        public ICollection<Amenity> Amenities { get; set; }

        public ICollection<Bed> Beds { get; set; }

        public int Capacity { get; set; }

        public string GettingAround { get; set; }

        public Guid Id { get; set; }

        public Page Page { get; set; }

        public Guid PageId { get; set; }

        public string Location { get; set; }

        public ICollection<Rule> Rules { get; set; }

        public ICollection<Space> Spaces { get; set; }

        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.ToTable("Hotels", "ModuleHotel");
        }
    }
}