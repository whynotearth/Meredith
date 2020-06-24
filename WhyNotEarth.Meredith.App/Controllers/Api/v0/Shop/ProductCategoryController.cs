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
    [ApiVersion("0")]
    [ProducesErrorResponseType(typeof(void))]
    [Route("api/v0/shop/tenant/{tenantSlug}/categories")]
    public class ProductCategoryController : ControllerBase
    {
        private readonly ProductCategoryService _productCategoryService;
        private readonly IUserService _userService;

        public ProductCategoryController(ProductCategoryService productCategoryService, IUserService userService)
        {
            _productCategoryService = productCategoryService;
            _userService = userService;
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<ProductCategoryResult>>> List(string tenantSlug)
        {
            var categories = await _productCategoryService.ListAsync(tenantSlug);

            var result = categories
                .Select(item => new ProductCategoryResult(item))
                .ToList();

            return Ok(result);
        }

        [Returns200]
        [HttpGet("{categoryId}")]
        public async Task<ActionResult<ProductCategoryResult>> Get(int categoryId)
        {
            var category = await _productCategoryService.GetAsync(categoryId);
            return Ok(new ProductCategoryResult(category));
        }

        [Returns200]
        [Returns401]
        [Returns403]
        [HttpPost("")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<ActionResult<ProductCategoryResult>> Create(string tenantSlug, ProductCategoryModel model)
        {
            var user = await _userService.GetUserAsync(User);

            var category = await _productCategoryService.CreateAsync(tenantSlug, model, user);

            return Ok(new ProductCategoryResult(category));
        }

        [Returns200]
        [Returns401]
        [Returns403]
        [Returns404]
        [HttpDelete("{categoryId}")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<ActionResult> Delete(int categoryId)
        {
            var user = await _userService.GetUserAsync(User);

            await _productCategoryService.DeleteAsync(categoryId, user);

            return NoContent();
        }

        [Returns204]
        [Returns401]
        [Returns403]
        [Returns404]
        [HttpPut("{categoryId}")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<ActionResult> Edit(string tenantSlug, int categoryId, ProductCategoryModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _productCategoryService.EditAsync(tenantSlug, categoryId, model, user);

            return NoContent();
        }
    }
}