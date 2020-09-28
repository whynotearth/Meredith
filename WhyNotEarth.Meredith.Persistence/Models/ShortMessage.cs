﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Twilio;

namespace WhyNotEarth.Meredith.Persistence.Models
{
    public class ShortMessageConfig : IEntityTypeConfiguration<ShortMessage>
    {
        public void Configure(EntityTypeBuilder<ShortMessage> builder)
        {
            builder.ToTable("ShortMessages", "public");
        }
    }
}