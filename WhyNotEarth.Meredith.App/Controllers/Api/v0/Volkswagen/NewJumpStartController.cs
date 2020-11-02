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
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Volkswagen;
using WhyNotEarth.Meredith.Volkswagen.Models;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    [ApiVersion("0")]
    [Route("api/v0/volkswagen/newjumpstart")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageVolkswagen)]
    public class NewJumpStartController : BaseController
    {
        private readonly IWebHostEnvironment _environment;
        private readonly NewJumpStartService _newJumpStartService;

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
        [HttpGet("stats")]
        public async Task<ActionResult<NewJumpStartOverAllStatsResult>> Stats([FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var (from, to) = Validate(fromDate, toDate);

            var stats = await _newJumpStartService.GetStatsAsync(from, to);

            return Ok(new NewJumpStartOverAllStatsResult(stats));
        }

        [Returns200]
        [HttpGet("stats/export")]
        public async Task<IActionResult> ExportStats([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var (from, to) = Validate(fromDate, toDate);

            var stats = await _newJumpStartService.GetStatsAsync(from, to);

            var result = NewJumpStartOverAllStatsCsvResult.Create(stats);

            return await Csv(result, _environment.IsDevelopment());
        }

        [Returns200]
        [HttpGet("{id}/stats")]
        public async Task<ActionResult<NewJumpStartSingleStatsResult>> Stats(int id, [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var (from, to) = Validate(fromDate, toDate);

            var stats = await _newJumpStartService.GetStatsAsync(from, to, id);

            return Ok(new NewJumpStartSingleStatsResult(stats));
        }

        [Returns200]
        [HttpGet("{id}/stats/export")]
        public async Task<IActionResult> StatsExport(int id, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var (from, to) = Validate(fromDate, toDate);

            var stats = await _newJumpStartService.GetStatsAsync(from, to, id);

            var result = NewJumpStartOverAllStatsCsvResult.Create(stats);

            return await Csv(result, _environment.IsDevelopment());
        }

        private (DateTime from, DateTime to) Validate(DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate is null || toDate is null)
            {
                throw new InvalidActionException();
            }

            return (fromDate.Value.Date, toDate.Value.Date);
        }
    }
}