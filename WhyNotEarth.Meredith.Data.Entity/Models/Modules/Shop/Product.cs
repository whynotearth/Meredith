using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop
{
    public class Product : Page
    {
        public int PriceId { get; set; }

        public Price Price { get; set; }

        public List<Variation> Variations { get; set; }

        public List<ProductLocationInventory> ProductLocationInventories { get; set; }
    }
}