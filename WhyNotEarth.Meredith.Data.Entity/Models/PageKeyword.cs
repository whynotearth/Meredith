using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class PageKeyword : IEntityTypeConfiguration<PageKeyword>
    {
        public int Id { get; set; }

        public string Keyword { get; set; }

        public void Configure(EntityTypeBuilder<PageKeyword> builder)
        {
        }
    }
}
