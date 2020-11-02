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
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Volkswagen;
using WhyNotEarth.Meredith.Volkswagen.Models;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
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

        [HttpGet("stats")]
        public async Task<List<DistributionGroupStatResult>> Stats()
        {
            var stats = await _recipientService.GetDistributionGroupStatsAsync();

            return stats.Select(item => new DistributionGroupStatResult(item)).ToList();
        }

        [HttpPut("")]
        public async Task<NoContentResult> Import(IFormFile file)
        {
            await _recipientService.ImportAsync(file.OpenReadStream());

            return NoContent();
        }

        [HttpGet("")]
        public async Task<List<string>> List()
        {
            return await _recipientService.GetDistributionGroupsAsync();
        }

        [HttpGet("{distributionGroupName}/recipients")]
        public async Task<List<RecipientResult>> ListRecipients(string distributionGroupName)
        {
            var recipients = await _recipientService.GetRecipientsAsync(distributionGroupName);

            return recipients.Select(item => new RecipientResult(item)).ToList();
        }

        [HttpPost("{distributionGroupName}/recipients")]
        public async Task<NoContentResult> Add(string distributionGroupName, RecipientModel model)
        {
            await _recipientService.AddAsync(distributionGroupName, model);

            return NoContent();
        }

        [Returns404]
        [HttpPut("{distributionGroupName}/recipients/{recipientId}")]
        public async Task<NoContentResult> Edit(int recipientId, RecipientModel model)
        {
            await _recipientService.EditAsync(recipientId, model);

            return NoContent();
        }

        [Returns404]
        [HttpDelete("{distributionGroupName}/recipients/{recipientId}")]
        public async Task<NoContentResult> Delete(int recipientId)
        {
            await _recipientService.DeleteAsync(recipientId);

            return NoContent();
        }

        [HttpGet("{distributionGroupName}/export")]
        public async Task<IActionResult> Export(string distributionGroupName)
        {
            var recipients = await _recipientService.GetRecipientsAsync(distributionGroupName);

            return await Csv(recipients.Select(item => new RecipientCsvExportResult(item)),
                _environment.IsDevelopment());
        }

        [HttpGet("{distributionGroupName}/stats")]
        public async Task<OverAllStatsResult> OverallStats(string distributionGroupName,
            [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var (from, to) = Validate(fromDate, toDate);

            var stats = await _recipientService.GetStatsAsync(from, to, distributionGroupName);

            return new OverAllStatsResult(stats);
        }

        [HttpGet("{distributionGroupName}/stats/export")]
        public async Task<IActionResult> ExportOverallUserStats(string distributionGroupName,
            [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var (from, to) = Validate(fromDate, toDate);

            var stats = await _recipientService.GetStatsAsync(from, to, distributionGroupName);

            var result = OverAllStatsCsvResult.Create(stats);

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