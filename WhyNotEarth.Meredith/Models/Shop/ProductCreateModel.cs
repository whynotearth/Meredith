using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Models.Shop
{
    public class ProductCreateModel: ProductModel
    {

        public List<ProductCreateLocationInventoryModel> ProductLocationInventories { get; set; } = null!;

        public List<VariationCreateModel> Variations { get; set; } = null!;
    }

    public class ProductCreateLocationInventoryModel: ProductLocationInventoryModel
    {
        
    }

    public class VariationCreateModel: VariationModel
    {

    }
}
