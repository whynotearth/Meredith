namespace WhyNotEarth.Meredith.Persistence.Models
{
    public class SendGridAccount
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        public string? Key { get; set; }

        public string ApiKey { get; set; } = null!;

        public string TemplateId { get; set; } = null!;

        public string FromEmail { get; set; } = null!;

        public string? FromEmailName { get; set; }

        public string? Bcc { get; set; }
    }
}