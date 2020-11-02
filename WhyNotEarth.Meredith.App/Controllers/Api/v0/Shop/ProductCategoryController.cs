using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.Shop;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Shop;
using WhyNotEarth.Meredith.Shop.Models;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Shop
{
    [ApiVersion("0")]
    [ProducesErrorResponseType(typeof(void))]
    [Route("api/v0/shop/tenant/{tenantSlug}/categories")]
    public class ProductCategoryController : BaseController
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

            return Ok(categories.Select(item => new ProductCategoryResult(item)));
        }

        [Returns200]
        [HttpGet("{categoryId}")]
        public async Task<ActionResult<ProductCategoryResult>> Get(int categoryId)
        {
            var category = await _productCategoryService.GetAsync(categoryId);

            return Ok(new ProductCategoryResult(category));
        }

        [Returns201]
        [HttpPost("")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<CreateResult> Create(string tenantSlug, ProductCategoryModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _productCategoryService.CreateAsync(tenantSlug, model, user);

            return Created();
        }

        [Returns200]
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