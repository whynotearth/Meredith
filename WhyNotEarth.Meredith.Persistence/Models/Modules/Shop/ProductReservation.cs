namespace WhyNotEarth.Meredith.Persistence.Models.Modules.Shop
{
    public class ProductReservation : Reservation
    {
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;
    }
}