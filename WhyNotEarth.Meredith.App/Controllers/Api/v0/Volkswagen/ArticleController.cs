using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [Route("api/v0/volkswagen/articles")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageVolkswagen)]
    public class ArticleController : ControllerBase
    {
        private readonly ArticleService _articleService;

        public ArticleController(ArticleService articleService)
        {
            _articleService = articleService;
        }

        [Returns200]
        [Returns404]
        [HttpPost("")]
        public async Task<IActionResult> Create(ArticleModel model)
        {
            await _articleService.CreateAsync(model.CategoryId, model.Date!.Value, model.Headline, model.Description,
                model.Price, model.EventDate, model.Image);

            return Ok();
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<JumpStartPreviewResult>>> GetAvailable(DateTime? date)
        {
            var availableArticles = await _articleService.GetAvailableArticles(date);

            var result = availableArticles.Select(item => new JumpStartPreviewResult(item.Key, item.Value)).ToList();

            return Ok(result);
        }

        [Returns204]
        [Returns404]
        [HttpPut("{articleId}")]
        public async Task<NoContentResult> Edit(int articleId, ArticleModel model)
        {
            await _articleService.EditAsync(articleId, model.CategoryId, model.Date!.Value, model.Headline,
                model.Description, model.Price, model.EventDate);

            return NoContent();
        }

        [Returns204]
        [Returns404]
        [HttpDelete("{articleId}")]
        public async Task<NoContentResult> Delete(int articleId)
        {
            await _articleService.DeleteAsync(articleId);

            return NoContent();
        }
    }
}