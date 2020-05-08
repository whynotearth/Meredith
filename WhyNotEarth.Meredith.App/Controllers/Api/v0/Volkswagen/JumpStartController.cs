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
        private readonly JumpStartPreviewService _jumpStartPreviewService;
        private readonly JumpStartService _jumpStartService;

        public JumpStartController(JumpStartService jumpStartService, JumpStartPreviewService jumpStartPreviewService)
        {
            _jumpStartService = jumpStartService;
            _jumpStartPreviewService = jumpStartPreviewService;
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<JumpStartResult>>> List()
        {
            var jumpStarts = await _jumpStartService.ListAsync();

            return Ok(jumpStarts.Select(item => new JumpStartResult(item)));
        }

        [Returns200]
        [HttpGet("{jumpStartId}/preview")]
        public async Task<FileContentResult> Preview(int jumpStartId, [FromQuery] List<int> articleIds)
        {
            var previewData = await _jumpStartPreviewService.CreatePreviewAsync(jumpStartId, articleIds);

            return File(previewData, "image/png", Guid.NewGuid() + ".png");
        }

        [Returns204]
        [HttpPut("{jumpStartId}")]
        public async Task<NoContentResult> Edit(int jumpStartId, JumpStartModel model)
        {
            await _jumpStartService.Edit(jumpStartId, model.DateTime!.Value, model.DistributionGroups,
                model.ArticleIds);

            return NoContent();
        }
    }
}