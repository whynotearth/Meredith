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
    [ApiVersion("0")]
    [Route("api/v0")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.Developer)]
    public class ProductCategoryController : ControllerBase
    {
        private readonly ProductCategoryService _productCategoryService;

        public ProductCategoryController(ProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        [Returns200]
        [HttpGet("tenant/{tenantSlug}/categories")]
        public async Task<ActionResult<List<ProductCategoryResult>>> List(string tenantSlug)
        {
            var categories = await _productCategoryService.ListAsync(tenantSlug);

            var result = categories
                .Select(item => new ProductCategoryResult(item))
                .ToList();

            return Ok(result);
        }

        [Returns200]
        [HttpGet("categories/{categoryId}")]
        public async Task<ActionResult<ProductCategoryResult>> Get(int categoryId)
        {
            var category = await _productCategoryService.GetAsync(categoryId);
            return Ok(new ProductCategoryResult(category));
        }

        [Returns200]
        [HttpPost("categories")]
        public async Task<ActionResult<ProductCategoryResult>> Create(ProductCategoryModel model)
        {
            var category = await _productCategoryService.CreateAsync(model);
            return Ok(new ProductCategoryResult(category));
        }

        [Returns200]
        [Returns404]
        [HttpDelete("categories/{categoryId}")]
        public async Task<ActionResult> Delete(int categoryId)
        {
            await _productCategoryService.DeleteAsync(categoryId);

            return NoContent();
        }

        [Returns204]
        [Returns404]
        [HttpPut("categories/{categoryId}")]
        public async Task<ActionResult> Edit(int categoryId, ProductCategoryModel model)
        {
            await _productCategoryService.EditAsync(categoryId, model);

            return NoContent();
        }
    }
}