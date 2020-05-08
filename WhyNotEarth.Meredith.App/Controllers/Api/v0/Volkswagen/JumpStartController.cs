using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
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
        private readonly MeredithDbContext _dbContext;
        private readonly JumpStartService _jumpStartService;

        public JumpStartController(JumpStartService jumpStartService, JumpStartPreviewService jumpStartPreviewService, MeredithDbContext dbContext)
        {
            _jumpStartService = jumpStartService;
            _jumpStartPreviewService = jumpStartPreviewService;
            _dbContext = dbContext;
        }

        [Returns200]
        [Returns400]
        [HttpPost("")]
        public async Task<IActionResult> Create(JumpStartModel model)
        {
            await _jumpStartService.Confirm(model.JumpStartId!.Value, model.DateTime!.Value, model.DistributionGroups,
                model.ArticleIds);

            return Ok();
        }

        [Returns200]
        [HttpGet("{jumpStartId}/preview")]
        public async Task<FileContentResult> Preview(int jumpStartId, [FromQuery]List<int>? articleIds)
        {
            var previewData = await _jumpStartPreviewService.CreatePreviewAsync(jumpStartId, articleIds);

            return File(previewData, "image/png", Guid.NewGuid() + ".png");
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<JumpStartResult>>> List()
        {
            var jumpStarts = await _dbContext.JumpStarts
                .Include(item => item.Articles)
                .ThenInclude(item => item.Category)
                .ThenInclude(item => item.Image)
                .Where(item => item.Status != JumpStartStatus.Sent)
                .ToListAsync();

            return Ok(jumpStarts.Select(item => new JumpStartResult(item)));
        }
    }
}