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

        [HttpGet("")]
        public async Task<List<ProductCategoryResult>> List(string tenantSlug)
        {
            var categories = await _productCategoryService.ListAsync(tenantSlug);

            return categories.Select(item => new ProductCategoryResult(item)).ToList();
        }

        [HttpGet("{categoryId}")]
        public async Task<ProductCategoryResult> Get(int categoryId)
        {
            var category = await _productCategoryService.GetAsync(categoryId);

            return new ProductCategoryResult(category);
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

        [Returns404]
        [HttpDelete("{categoryId}")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<NoContentResult> Delete(int categoryId)
        {
            var user = await _userService.GetUserAsync(User);

            await _productCategoryService.DeleteAsync(categoryId, user);

            return NoContent();
        }

        [Returns204]
        [Returns404]
        [HttpPut("{categoryId}")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<NoContentResult> Edit(string tenantSlug, int categoryId, ProductCategoryModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _productCategoryService.EditAsync(tenantSlug, categoryId, model, user);

            return NoContent();
        }
    }
}