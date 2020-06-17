using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen;
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
        private readonly IWebHostEnvironment _environment;

        public NewJumpStartController(NewJumpStartService newJumpStartService, IWebHostEnvironment environment)
        {
            _newJumpStartService = newJumpStartService;
            _environment = environment;
        }

        [Returns201]
        [HttpPost("")]
        public async Task<CreateResult> Create(NewJumpStartModel model)
        {
            await _newJumpStartService.CreateAsync(model);

            return Created();
        }

        [Returns200]
        [HttpGet("stats")]
        public async Task<ActionResult<NewJumpStartStatsResult>> Stats([FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            var stats = await _newJumpStartService.GetStatsAsync(fromDate.Date, toDate.Date);

            return Ok(new NewJumpStartStatsResult(stats));
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<NewJumpStartResult>>> List()
        {
            var newJumpStarts = await _newJumpStartService.ListAsync();

            return Ok(newJumpStarts.Select(item => new NewJumpStartResult(item)));
        }

        [Returns204]
        [HttpPut("{id}")]
        public async Task<ActionResult<NewJumpStartResult>> Edit(int id, NewJumpStartModel model)
        {
            await _newJumpStartService.EditAsync(id, model);

            return NoContent();
        }

        [Returns200]
        [HttpGet("stats/export")]
        public async Task<IActionResult> ExportUserStats([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var result = new List<OverAllStatsCsvResult>();

            var stats = await _newJumpStartService.GetUserStatsAsync(fromDate.Date, toDate.Date);
            result.AddRange(stats.Select(item => new OverAllStatsCsvResult(OverAllStatsTypeCsvResult.User, item)));

            stats = await _newJumpStartService.GetOpenStatsAsync(fromDate.Date, toDate.Date);
            result.AddRange(stats.Select(item => new OverAllStatsCsvResult(OverAllStatsTypeCsvResult.Open, item)));

            stats = await _newJumpStartService.GetClickStatsAsync(fromDate.Date, toDate.Date);
            result.AddRange(stats.Select(item => new OverAllStatsCsvResult(OverAllStatsTypeCsvResult.Click, item)));

            return await Csv(result, _environment.IsDevelopment());
        }
    }
}