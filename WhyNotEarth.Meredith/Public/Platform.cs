namespace WhyNotEarth.Meredith.Public
{
    public class Platform
    {
        public int Id { get; set; }

        public Company? Company { get; set; }

        public int? CompanyId { get; set; }

        public string Domain { get; set; } = null!;

        public string Name { get; set; } = null!;

        public decimal? SalesCut { get; set; }
    }
}