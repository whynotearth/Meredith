namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public class ProductReservation : Reservation
    {
        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}