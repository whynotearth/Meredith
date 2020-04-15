using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class Keyword : IEntityTypeConfiguration<Keyword>
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }
        
        [ForeignKey("PageId")]
        [Required]
        public Page Page { get; set; }

        public void Configure(EntityTypeBuilder<Keyword> builder)
        {

        }
    }
}
