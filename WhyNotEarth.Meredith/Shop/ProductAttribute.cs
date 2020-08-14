namespace WhyNotEarth.Meredith.Shop
{
    public class ProductAttribute
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int PriceId { get; set; }

        public Price Price { get; set; } = null!;
    }
}