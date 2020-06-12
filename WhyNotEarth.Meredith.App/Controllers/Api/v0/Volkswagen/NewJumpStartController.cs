using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Volkswagen;
using WhyNotEarth.Meredith.Volkswagen.Models;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [Route("api/v0/volkswagen/newjumpstart")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageVolkswagen)]
    public class NewJumpStartController : BaseController
    {
        private readonly NewJumpStartService _newJumpStartService;

        public NewJumpStartController(NewJumpStartService newJumpStartService)
        {
            _newJumpStartService = newJumpStartService;
        }

        [Returns201]
        [HttpPost("")]
        public async Task<CreateResult> Create(NewJumpStartModel model)
        {
            await _newJumpStartService.CreateAsync(model);

            return Created();
        }

        [Returns201]
        [HttpPost("{date}/attachment")]
        public async Task<CreateResult> UploadPdf(IFormFile file, DateTime date)
        {
            var permittedExtensions = new[] {".pdf"};

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                throw new InvalidActionException("Invalid attachment file type");
            }

            await _newJumpStartService.SaveAttachmentAsync(date.Date, file.OpenReadStream());

            return Created();
        }

        [Returns200]
        [HttpGet("stats")]
        public async Task<ActionResult<JumpStartStatsResult>> Stats([FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            var stats = await _newJumpStartService.GetStatsAsync(fromDate.Date, toDate.Date);

            return Ok(new JumpStartStatsResult(stats));
        }
    }
}