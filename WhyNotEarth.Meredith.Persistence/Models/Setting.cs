namespace WhyNotEarth.Meredith.Persistence.Models
{
    public class Setting
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        public string? Value { get; set; }
    }
}