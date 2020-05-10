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
        private readonly JumpStartPlanService _jumpStartPlanService;

        public ArticleController(ArticleService articleService, JumpStartPlanService jumpStartPlanService)
        {
            _articleService = articleService;
            _jumpStartPlanService = jumpStartPlanService;
        }

        [Returns200]
        [Returns404]
        [HttpPost("")]
        public async Task<IActionResult> Create(ArticleModel model)
        {
            await _articleService.CreateAsync(model.CategorySlug, model.Date!.Value.Date, model.Headline,
                model.Description,
                model.Price, model.EventDate, model.Image);

            return Ok();
        }

        [Returns204]
        [Returns404]
        [HttpPut("{articleId}")]
        public async Task<NoContentResult> Edit(int articleId, ArticleModel model)
        {
            await _articleService.EditAsync(articleId, model.CategorySlug, model.Date!.Value.Date, model.Headline,
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

        [Returns200]
        [HttpGet("dailyplan")]
        public async Task<ActionResult<List<ArticleDailyResult>>> DailyPlan()
        {
            var dailyArticles = await _jumpStartPlanService.GetPlanAsync();

            return Ok(dailyArticles.OrderBy(item => item.Key)
                .Select(item => new ArticleDailyResult(item.Key, item.Value)));
        }
    }
}