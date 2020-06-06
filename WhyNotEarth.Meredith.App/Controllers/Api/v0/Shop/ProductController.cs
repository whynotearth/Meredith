using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Results.Api.v0.Shop;
using WhyNotEarth.Meredith.Models.Shop;
using WhyNotEarth.Meredith.Shop;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Shop
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [Route("api/v0/shop/products")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.Developer)]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [Returns200]
        [HttpPost("")]
        public async Task<ActionResult<ShopProductResult>> Create(ProductCreateModel model)
        {
            var product = await _productService.CreateAsync(model);

            return Ok(new ShopProductResult(product));
        }

        [Returns204]
        [Returns404]
        [HttpPut("")]
        public async Task<IActionResult> Edit(ProductEditModel model)
        {
            await _productService.EditAsync(model);

            return NoContent();
        }

        [Returns204]
        [Returns404]
        [HttpDelete("{categoryId}/{productId}")]
        public async Task<IActionResult> Delete(int categoryId, int productId)
        {
            await _productService.DeleteAsync(categoryId, productId);

            return NoContent();
        }

        [Returns200]
        [Returns404]
        [HttpGet("{categoryId}/{productId}")]
        public async Task<ActionResult<ShopProductResult>> Get(int categoryId, int productId)
        {
            var product = await _productService.GetAsync(categoryId, productId);

            return Ok(new ShopProductResult(product));
        }

        [Returns200]
        [HttpGet("{categoryId}")]
        public async Task<ActionResult<List<ShopProductResult>>> List(int categoryId)
        {
            var products = await _productService.ListAsync(categoryId);

            return Ok(products.Select(item => new ShopProductResult(item)).ToList());
        }
    }
}
