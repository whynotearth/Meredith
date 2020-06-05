using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Models.Shop
{
    public class ProductCreateModel: ProductModel
    {
        public List<ProductLocationInventoryCreateModel> ProductLocationInventories { get; set; } = null!;

        public List<VariationCreateModel> Variations { get; set; } = null!;

        public List<ProductAttributeCreateModel> ProductAttributes { get; set; } = null!;
    }

    public class ProductLocationInventoryCreateModel: ProductLocationInventoryModel
    {

    }

    public class VariationCreateModel: VariationModel
    {

    }

    public class ProductAttributeCreateModel : ProductAttributeModel
    {

    }
}
