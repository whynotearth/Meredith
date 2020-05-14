using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.App.Models.Api.v0.Public;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Products;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [ApiVersion("0")]
    [Route("api/v0")]
    [ProducesErrorResponseType(typeof(void))]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("tenants/{tenantSlug}/categories")]
        public async Task<ActionResult<List<CategoryResult>>> Categories(string tenantSlug)
        {
            var productCategories = await _productService.GetCategoryListAsync(tenantSlug);

            return Ok(productCategories.Select(item => new CategoryResult(item)).ToList());
        }

        [HttpGet("tenants/{tenantSlug}/categories/{categorySlug}/products")]
        public async Task<ActionResult<List<ProductResult>>> Products(string tenantSlug, string categorySlug)
        {
            var products = await _productService.GetProductListAsync(tenantSlug, categorySlug);

            return Ok(products.Select(item => new ProductResult(item)).ToList());
        }

        [HttpGet("products/{productId}")]
        public async Task<ActionResult<ProductResult>> Products(int productId)
        {
            var product = await _productService.GetProductAsync(productId);

            return Ok(new ProductResult(product));
        }

        [Returns204]
        [Returns404]
        [HttpPut("products/{productId}")]
        public async Task<NoContentResult> Edit(int productId, ProductModel model)
        {
            await _productService.EditAsync(productId, model.TenantId, model.CategoryId, model.Name, model.Description,
                model.Price, model.Currency, model.Images ?? new List<ProductImage>());

            return NoContent();
        }

    }
}