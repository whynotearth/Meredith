using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
    [Route("api/v0/volkswagen/distributiongroups")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageVolkswagen)]
    public class DistributionGroupController : BaseController
    {
        private readonly IWebHostEnvironment _environment;
        private readonly RecipientService _recipientService;

        public DistributionGroupController(RecipientService recipientService, IWebHostEnvironment environment)
        {
            _recipientService = recipientService;
            _environment = environment;
        }

        [Returns200]
        [HttpGet("stats")]
        public async Task<ActionResult<DistributionGroupStatResult>> Stats()
        {
            var stats = await _recipientService.GetDistributionGroupStatsAsync();

            return Ok(stats.Select(item => new DistributionGroupStatResult(item)));
        }

        [Returns204]
        [HttpPut("")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            await _recipientService.ImportAsync(file.OpenReadStream());

            return NoContent();
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<string>>> List()
        {
            var result = await _recipientService.GetDistributionGroupsAsync();

            return Ok(result);
        }

        [Returns200]
        [HttpGet("{distributionGroupName}/recipients")]
        public async Task<ActionResult<List<RecipientResult>>> ListRecipients(string distributionGroupName)
        {
            var recipients = await _recipientService.GetRecipientsAsync(distributionGroupName);

            var result = recipients.Select(item => new RecipientResult(item)).ToList();

            return Ok(result);
        }

        [Returns204]
        [HttpPost("{distributionGroupName}/recipients")]
        public async Task<NoContentResult> Add(string distributionGroupName, RecipientModel model)
        {
            await _recipientService.AddAsync(distributionGroupName, model);

            return NoContent();
        }

        [Returns204]
        [Returns404]
        [HttpPut("{distributionGroupName}/recipients/{recipientId}")]
        public async Task<NoContentResult> Edit(int recipientId, RecipientModel model)
        {
            await _recipientService.EditAsync(recipientId, model);

            return NoContent();
        }

        [Returns204]
        [Returns404]
        [HttpDelete("{distributionGroupName}/recipients/{recipientId}")]
        public async Task<NoContentResult> Delete(int recipientId)
        {
            await _recipientService.DeleteAsync(recipientId);

            return NoContent();
        }

        [Returns200]
        [HttpGet("{distributionGroupName}/export")]
        public async Task<IActionResult> Export(string distributionGroupName)
        {
            var recipients = await _recipientService.GetRecipientsAsync(distributionGroupName);

            return await Csv(recipients.Select(item => new RecipientCsvExportResult(item)),
                _environment.IsDevelopment());
        }

        [Returns200]
        [HttpGet("{distributionGroupName}/stats")]
        public async Task<ActionResult<List<MemoStatResult>>> OverallStats(string distributionGroupName,
            [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var stats = await _recipientService.GetStatsAsync(fromDate.Date, toDate.Date, distributionGroupName);

            return Ok(new MemoOverAllStatsResult(stats));
        }

        [Returns200]
        [HttpGet("{distributionGroupName}/stats/export")]
        public async Task<IActionResult> ExportOverallUserStats(string distributionGroupName,
            [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var result = new List<OverAllStatsCsvResult>();

            var stats = await _recipientService.GetUserStatsAsync(fromDate.Date, toDate.Date, distributionGroupName);
            result.AddRange(stats.Select(item => new OverAllStatsCsvResult(OverAllStatsTypeCsvResult.User, item)));

            stats = await _recipientService.GetOpenStatsAsync(fromDate.Date, toDate.Date, distributionGroupName);
            result.AddRange(stats.Select(item => new OverAllStatsCsvResult(OverAllStatsTypeCsvResult.Open, item)));

            stats = await _recipientService.GetClickStatsAsync(fromDate.Date, toDate.Date, distributionGroupName);
            result.AddRange(stats.Select(item => new OverAllStatsCsvResult(OverAllStatsTypeCsvResult.Click, item)));

            return await Csv(result, _environment.IsDevelopment());
        }
    }
}