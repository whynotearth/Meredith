using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Models.Api.v0.Public;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Products;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [ApiVersion("0")]
    [Route("api/v0/foodouken")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageFoodouken)]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
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