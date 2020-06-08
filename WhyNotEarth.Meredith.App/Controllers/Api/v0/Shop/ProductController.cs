using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Results.Api.v0.Shop;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Models;
using WhyNotEarth.Meredith.Shop;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Shop
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [Route("api/v0/shop/categories/{categoryId}/products")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.Developer)]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly IUserManager _userManager;

        public ProductController(ProductService productService, IUserManager userManager)
        {
            _productService = productService;
            _userManager = userManager;
        }

        [Returns200]
        [HttpPost("")]
        public async Task<ActionResult<ShopProductResult>> Create(int categoryId, ProductModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            var product = await _productService.CreateAsync(categoryId, model, user);

            return Ok(new ShopProductResult(product));
        }

        [Returns204]
        [Returns404]
        [HttpPut("{productId}")]
        public async Task<IActionResult> Edit(int productId, int categoryId, ProductModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            await _productService.EditAsync(productId, categoryId, model, user);

            return NoContent();
        }

        [Returns204]
        [Returns404]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var user = await _userManager.GetUserAsync(User);

            await _productService.DeleteAsync(productId, user);

            return NoContent();
        }

        [Returns200]
        [Returns404]
        [HttpGet("{productId}")]
        public async Task<ActionResult<ShopProductResult>> Get(int productId)
        {
            var product = await _productService.GetAsync(productId);

            return Ok(new ShopProductResult(product));
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<ShopProductResult>>> List(int categoryId)
        {
            var products = await _productService.ListAsync(categoryId);

            return Ok(products.Select(item => new ShopProductResult(item)).ToList());
        }
    }
}
