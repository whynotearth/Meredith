namespace WhyNotEarth.Meredith.BrowTricks
{
    public class Disclosure
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; } = null!;

        public string Value { get; set; } = null!;
    }
}