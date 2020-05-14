using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Products;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [ApiVersion("0")]
    [Route("api/v0/tenants/{tenantSlug}")]
    [ProducesErrorResponseType(typeof(void))]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsService _productService;

        public ProductsController(ProductsService productService)
        {
            _productService = productService;
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryResult>>> Categories(string tenantSlug)
        {
            var productCategories = await _productService.GetCategoryListAsync(tenantSlug);

            return Ok(productCategories.Select(item => new CategoryResult(item)).ToList());
        }

        [HttpGet("categories/{categorySlug}/products")]
        public async Task<ActionResult<List<ProductResult>>> Products(string tenantSlug, string categorySlug)
        {
            var products = await _productService.GetProductListAsync(tenantSlug, categorySlug);

            return Ok(products.Select(item => new ProductResult(item)).ToList());
        }
    }
}