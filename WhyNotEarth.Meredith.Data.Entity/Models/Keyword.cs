using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class Keyword : IEntityTypeConfiguration<Keyword>
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Word { get; set; }

        public void Configure(EntityTypeBuilder<Keyword> builder)
        {

        }
    }
}
