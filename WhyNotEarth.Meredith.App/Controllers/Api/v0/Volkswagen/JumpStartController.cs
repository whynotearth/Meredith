using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen;
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
        private readonly JumpStartService _jumpStartService;
        private readonly JumpStartPreviewService _jumpStartPreviewService;

        public JumpStartController(JumpStartService jumpStartService, JumpStartPreviewService jumpStartPreviewService)
        {
            _jumpStartService = jumpStartService;
            _jumpStartPreviewService = jumpStartPreviewService;
        }

        [Returns200]
        [Returns400]
        [HttpPost("")]
        public async Task<IActionResult> Create(JumpStartModel model)
        {
            await _jumpStartService.CreateAsync(model.DateTime, model.DistributionGroups, model.PostIds);

            return Ok();
        }

        
        [Returns200]
        [HttpGet("preview")]
        public async Task<FileContentResult> Preview([FromQuery]List<int> postIds)
        {
            var previewData = await _jumpStartPreviewService.CreatePreviewAsync(postIds);

            return File(previewData, "image/png", Guid.NewGuid() + ".png");
        }
    }
}