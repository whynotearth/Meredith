using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Page;
using WhyNotEarth.Meredith.Models.Page;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [ApiVersion("0")]
    [Route("api/v0")]
    [ProducesErrorResponseType(typeof(void))]
    public class PageCategoryController : ControllerBase
    {
        private readonly PageCategoryService _pageCategoryService;

        public PageCategoryController(PageCategoryService pageCategoryService)
        {
            _pageCategoryService = pageCategoryService;
        }

        [Returns200]
        [HttpGet("tenant/{tenantSlug}/categories")]
        public async Task<ActionResult<List<PageCategoryResult>>> List(string tenantSlug)
        {
            var categories = await _pageCategoryService.ListAsync(tenantSlug);

            var result = categories
                .Select(x => new PageCategoryResult(x.Id, x.Slug, x.Name, x.Image.Url, x.Description))
                .ToList();

            return Ok(result);
        }

        [Returns200]
        [HttpPost("categories/{categoryId}")]
        public async Task<ActionResult<PageCategoryResult>> Create(int categoryId)
        {
            var category = await _pageCategoryService.GetAsync(categoryId);
            return Ok(new PageCategoryResult(category.Id, category.Slug, category.Name, category.Image.Url, category.Description));
        }

        [Returns200]
        [HttpPost("categories")]
        public async Task<ActionResult<PageCategoryResult>> Create(PageCategoryCreateModel categoryCreateModel)
        {
            return Ok(await _pageCategoryService.CreateAsync(categoryCreateModel));
        }

        [Returns200]
        [Returns404]
        [HttpDelete("categories/{categoryId}")]
        public async Task<ActionResult> Delete(int categoryId)
        {
            await _pageCategoryService.DeleteAsync(categoryId);

            return NoContent();
        }

        [Returns204]
        [Returns404]
        [HttpPut("categories")]
        public async Task<ActionResult> Edit(PageCategoryEditModel pageCategoryEditModel)
        {
            await _pageCategoryService.EditAsync(pageCategoryEditModel);

            return NoContent();
        }
    }
}