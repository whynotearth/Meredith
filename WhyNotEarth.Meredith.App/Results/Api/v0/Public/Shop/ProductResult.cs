using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.Shop
{
  public class ProductResult
  {
    public int Id { get; set; }

    public int PageId { get; set; }

    public int PriceId { get; set; }

    public List<ProductLocationInventoryResult> ProductLocationInventories { get; set; } = null!;

    public List<VariationResult> Variations { get; set; } = null!;

    public ProductResult()
    {
    }

    public ProductResult(Product product)
    {
      PageId = product.PageId;
      PriceId = product.PriceId;
      ProductLocationInventories = product.ProductLocationInventories
          .Select(item => new ProductLocationInventoryResult(item))
          .ToList();
      Variations = product.Variations
          .Select(item => new VariationResult(item))
          .ToList();
    }

    public class ProductLocationInventoryResult
    {
      public int Id { get; set; }

      public int LocationId { get; set; }

      public int Count { get; set; }

      public ProductLocationInventoryResult()
      {
      }

      public ProductLocationInventoryResult(ProductLocationInventory productLocationInventory)
      {
        Id = productLocationInventory.Id;
        LocationId = productLocationInventory.LocationId;
        Count = productLocationInventory.Count;
      }
    }

    public class VariationResult
    {
      public int Id { get; set; }

      public string Name { get; set; } = null!;

      public VariationResult()
      {
      }

      public VariationResult(Variation variation)
      {
        Name = variation.Name;
        Id = variation.Id;
      }
    }
  }
}
