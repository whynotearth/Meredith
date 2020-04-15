using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.Products
{
    public class ProductResult
    {
        public int Id { get; }

        public string Name { get; }

        public string Description { get; }

        public List<string>? Images { get; }

        public decimal Price { get; }

        public string Currency { get; }

        public ProductResult(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            Images = product.Images?.OrderBy(item => item.Order)?.Select(item => item.Url)?.ToList();
            Price = product.Price;
            Currency = product.Currency;
        }
    }
}