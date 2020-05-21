using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.Email;
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
        private readonly EmailRecipientService _emailRecipientService;
        private readonly JumpStartPreviewService _jumpStartPreviewService;
        private readonly JumpStartService _jumpStartService;

        public JumpStartController(JumpStartService jumpStartService, JumpStartPreviewService jumpStartPreviewService,
            JumpStartPlanService jumpStartPlanService, EmailRecipientService emailRecipientService)
        {
            _jumpStartService = jumpStartService;
            _jumpStartPreviewService = jumpStartPreviewService;
            _jumpStartPlanService = jumpStartPlanService;
            _emailRecipientService = emailRecipientService;
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
        public async Task<string> Preview(DateTime date, [FromQuery] List<int> articleIds)
        {
            var previewData = await _jumpStartPreviewService.CreatePreviewAsync(date.Date, articleIds);

            return "data:image/png;base64," + Convert.ToBase64String(previewData);
        }

        [Returns204]
        [HttpPost("")]
        public async Task<NoContentResult> CreateOrEdit(JumpStartModel model)
        {
            await _jumpStartService.CreateOrEditAsync(model.Id, model.DateTime!.Value, model.DistributionGroups!,
                model.ArticleIds!);

            return NoContent();
        }

        [Returns200]
        [HttpGet("stats")]
        public async Task<ActionResult<List<JumpStartStatResult>>> Stats()
        {
            var stats = await _jumpStartService.GetStatsAsync();

            return Ok(stats.Select(item => new JumpStartStatResult(item)));
        }

        [Returns200]
        [Returns404]
        [HttpGet("{jumpStartId}/stats")]
        public async Task<ActionResult<List<JumpStartStatDetailResult>>> DetailStats(int jumpStartId)
        {
            var memoInfo = await _jumpStartService.GetStatsAsync(jumpStartId);
            var result = new JumpStartStatDetailResult(memoInfo);

            var detailStats = await _emailRecipientService.GetJumpStartDetailStatsAsync(jumpStartId);

            result.NotOpened.AddRange(detailStats.NotOpenedList.Select(item => new EmailRecipientResult(item)));

            result.Opened.AddRange(detailStats.OpenedList.Select(item => new EmailRecipientResult(item)));

            return Ok(result);
        }
    }
}