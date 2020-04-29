using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    // Merge this with Company entity?
    public class SendGridAccount : IEntityTypeConfiguration<SendGridAccount>
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public string ApiKey { get; set; }

        public string TemplateId { get; set; }

        public string FromEmail { get; set; }

        public string FromEmailName { get; set; }

        public string Bcc { get; set; }

        public void Configure(EntityTypeBuilder<SendGridAccount> builder)
        {
            builder.Property(b => b.ApiKey).IsRequired();
        }
    }
}