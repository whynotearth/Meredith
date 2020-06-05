using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.Models.Shop
{
    public class ProductEditModel: ProductModel
    {
        public int Id { get; set; }

        public List<ProductLocationInventoryEditModel> ProductLocationInventories { get; set; } = null!;

        public List<VariationEditModel> Variations { get; set; } = null!;

        public List<ProductAttributeEditModel> ProductAttributes { get; set; } = null!;
    }

    public class ProductLocationInventoryEditModel: ProductLocationInventoryModel
    {
        public int Id { get; set; }
    }

    public class VariationEditModel: VariationModel
    {
        public int Id { get; set; }

        [Required]
        public int PriceId { get; set; }
    }

    public class ProductAttributeEditModel : ProductAttributeModel
    {
        public int Id { get; set; }

        [Required]
        public int PriceId { get; set; }
    }
}
