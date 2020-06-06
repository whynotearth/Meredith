using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using WhyNotEarth.Meredith.Validation;

namespace WhyNotEarth.Meredith.Models.Shop
{
    public abstract class ProductModel
    {
        [Required]
        public int PageId { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [NotNull]
        [Mandatory]
        public string Name { get; set; } = null!;
    }

    public class ProductLocationInventoryModel
    {
        [Required]
        public int LocationId { get; set; }

        [Required]
        public int Count { get; set; }
    }

    public class VariationModel
    {
        [Required]
        [NotNull]
        [Mandatory]
        public string Name { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }
    }

    public class ProductAttributeModel
    {
        [Required]
        [NotNull]
        [Mandatory]
        public string Name { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }
    }
}
