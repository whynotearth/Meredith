using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Shop
{
    public class ProductModel
    {
        [Required]
        public int PageId { get; set; }

        [Required]
        public int PriceId { get; set; }

        public List<ProductLocationInventoryModel> ProductLocationInventories { get; set; } = null!;

        public List<VariationModel> Variations { get; set; } = null!;
    }

    public class ProductLocationInventoryModel
    {
        public int Id { get; set; }

        [Required]
        public int LocationId { get; set; }

        public int Count { get; set; }
    }

    public class VariationModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;
    }
}
