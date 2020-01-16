namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Site : IEntityTypeConfiguration<Site>
    {
        public Company Company { get; set; }

        public int CompanyId { get; set; }

        public string Domain { get; set; }

        public int Id { get; set; }

        public void Configure(EntityTypeBuilder<Site> builder)
        {

        }
    }
}