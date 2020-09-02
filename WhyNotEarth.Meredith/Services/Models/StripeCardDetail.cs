namespace WhyNotEarth.Meredith.Services.Models
{
    public class StripeCardDetail
    {
        public string Id { get; set; } = null!;

        public string Last4 { get; set; } = null!;

        public string Brand { get; set; } = null!;

        public byte ExpirationMonth { get; set; }

        public ushort ExpirationYear { get; set; }
    }
}