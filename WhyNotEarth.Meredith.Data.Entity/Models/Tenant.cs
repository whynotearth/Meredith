﻿using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class Tenant : IEntityTypeConfiguration<Tenant>
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public ICollection<Page> Pages { get; set; }

        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
        }
    }
}
