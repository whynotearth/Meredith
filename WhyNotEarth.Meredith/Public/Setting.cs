namespace WhyNotEarth.Meredith.Public
{
    public class Setting
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        public string? Value { get; set; }
    }
}