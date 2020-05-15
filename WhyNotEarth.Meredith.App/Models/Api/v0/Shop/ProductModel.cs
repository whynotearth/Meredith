using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;

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

        public ProductModel()
        {
        }

        public ProductModel(Product product)
        {
            PageId = product.PageId;
            PriceId = product.PriceId;
            ProductLocationInventories = product.ProductLocationInventories
                .Select(item => new ProductLocationInventoryModel(item))
                .ToList();
            Variations = product.Variations
                .Select(item => new VariationModel(item))
                .ToList();
        }
    }

    public class ProductLocationInventoryModel
    {
        public int Id { get; set; }

        [Required]
        public int LocationId { get; set; }

        public int Count { get; set; }

        public ProductLocationInventoryModel()
        {
        }

        public ProductLocationInventoryModel(ProductLocationInventory productLocationInventory)
        {
            Id = productLocationInventory.Id;
            LocationId = productLocationInventory.LocationId;
            Count = productLocationInventory.Count;
        }
    }

    public class VariationModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public VariationModel()
        {
        }

        public VariationModel(Variation variation)
        {
            Name = variation.Name;
            Id = variation.Id;
        }
    }
}
