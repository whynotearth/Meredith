using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.Models.Shop
{
    public class ProductEditModel: ProductModel
    {
        public int Id { get; set; }

        public List<ProductEditLocationInventoryModel> ProductLocationInventories { get; set; } = null!;

        public List<VariationEditModel> Variations { get; set; } = null!;
    }

    public class ProductEditLocationInventoryModel: ProductLocationInventoryModel
    {
        public int Id { get; set; }
    }

    public class VariationEditModel: VariationModel
    {
        public int Id { get; set; }

        [Required]
        public int PriceId { get; set; }
    }
}
