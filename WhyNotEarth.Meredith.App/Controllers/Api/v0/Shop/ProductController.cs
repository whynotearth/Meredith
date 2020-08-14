using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.Shop;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Models;
using WhyNotEarth.Meredith.Shop;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Shop
{
    [ApiVersion("0")]
    [ProducesErrorResponseType(typeof(void))]
    [Route("api/v0/shop/categories/{categoryId}/products")]
    public class ProductController : BaseController
    {
        private readonly ProductService _productService;
        private readonly IUserService _userService;

        public ProductController(ProductService productService, IUserService userService)
        {
            _productService = productService;
            _userService = userService;
        }

        [Returns200]
        [Returns401]
        [Returns403]
        [HttpPost("")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<CreateResult> Create(int categoryId, ProductModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _productService.CreateAsync(categoryId, model, user);

            return Created();
        }

        [Returns204]
        [Returns401]
        [Returns403]
        [Returns404]
        [HttpPut("{productId}")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<NoContentResult> Edit(int productId, int categoryId, ProductModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _productService.EditAsync(productId, categoryId, model, user);

            return NoContent();
        }

        [Returns204]
        [Returns401]
        [Returns403]
        [Returns404]
        [HttpDelete("{productId}")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<NoContentResult> Delete(int productId)
        {
            var user = await _userService.GetUserAsync(User);

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