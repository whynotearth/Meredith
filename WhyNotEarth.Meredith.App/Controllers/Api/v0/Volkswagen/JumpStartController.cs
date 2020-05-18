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
    [Route("api/v0/volkswagen/jumpstart")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageVolkswagen)]
    public class JumpStartController : ControllerBase
    {
        private readonly JumpStartPlanService _jumpStartPlanService;
        private readonly JumpStartPreviewService _jumpStartPreviewService;
        private readonly JumpStartService _jumpStartService;

        public JumpStartController(JumpStartService jumpStartService, JumpStartPreviewService jumpStartPreviewService,
            JumpStartPlanService jumpStartPlanService)
        {
            _jumpStartService = jumpStartService;
            _jumpStartPreviewService = jumpStartPreviewService;
            _jumpStartPlanService = jumpStartPlanService;
        }

        [Returns200]
        [HttpGet("dailyplan")]
        public async Task<ActionResult<List<JumpStartPlanResult>>> DailyPlan()
        {
            var dailyPlan = await _jumpStartPlanService.GetAsync();

            return Ok(dailyPlan.Select(item => new JumpStartPlanResult(item)));
        }

        [Returns200]
        [HttpGet("{date}/preview")]
        public async Task<FileContentResult> Preview(DateTime date, [FromQuery] List<int> articleIds)
        {
            var previewData = await _jumpStartPreviewService.CreatePreviewAsync(date.Date, articleIds);

            return File(previewData, "image/png", Guid.NewGuid() + ".png");
        }

        [Returns204]
        [HttpPost("")]
        public async Task<NoContentResult> CreateOrEdit(JumpStartModel model)
        {
            await _jumpStartService.CreateOrEditAsync(model.Id, model.DateTime!.Value, model.DistributionGroups!,
                model.ArticleIds!);

            return NoContent();
        }
    }
}