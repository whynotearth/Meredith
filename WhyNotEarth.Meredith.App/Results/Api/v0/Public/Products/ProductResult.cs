using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.Products
{
    public class ProductResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Image { get; set; }

        public string Description { get; set; }

        public List<string> Images { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public ProductResult(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            Images = product.Images.OrderBy(item => item.Order).Select(item => item.Url).ToList();
            Price = product.Price;
            Currency = product.Currency;
        }
    }
}