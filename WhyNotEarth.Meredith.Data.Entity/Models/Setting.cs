using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class Setting : IEntityTypeConfiguration<Setting>
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; }

        public string Value { get; set; }

        public void Configure(EntityTypeBuilder<Setting> builder)
        {
        }
    }
}