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
using WhyNotEarth.Meredith.Emails;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Volkswagen;
using WhyNotEarth.Meredith.Volkswagen.Models;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    [ApiVersion("0")]
    [Route("api/v0/volkswagen/memos")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageVolkswagen)]
    public class MemoController : BaseController
    {
        private readonly EmailRecipientService _emailRecipientService;
        private readonly IWebHostEnvironment _environment;
        private readonly MemoService _memoService;

        public MemoController(MemoService memoService, EmailRecipientService emailRecipientService,
            IWebHostEnvironment environment)
        {
            _memoService = memoService;
            _emailRecipientService = emailRecipientService;
            _environment = environment;
        }

        [HttpPost("")]
        public async Task<CreateResult> Create(MemoModel model)
        {
            await _memoService.CreateAsync(model);

            return Created();
        }

        [HttpGet("stats")]
        public async Task<List<MemoStatResult>> Stats()
        {
            var stats = await _memoService.GetStatsAsync();

            return stats.Select(item => new MemoStatResult(item)).ToList();
        }

        [Returns404]
        [HttpGet("{memoId}/stats")]
        public async Task<MemoStatDetailResult> DetailStats(int memoId)
        {
            var memoInfo = await _memoService.GetStatsAsync(memoId);
            var result = new MemoStatDetailResult(memoInfo);

            var detailStats = await _emailRecipientService.GetMemoDetailStatsAsync(memoId);

            result.NotOpened.AddRange(detailStats.NotOpenedList.Select(item => new EmailRecipientResult(item)));

            result.Opened.AddRange(detailStats.OpenedList.Select(item => new EmailRecipientResult(item)));

            return result;
        }

        [HttpGet("overallstats")]
        public async Task<OverAllStatsResult> OverallStats([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var (from, to) = Validate(fromDate, toDate);

            var stats = await _memoService.GetStatsAsync(from, to);

            return new OverAllStatsResult(stats);
        }

        [HttpGet("overallstats/export")]
        public async Task<IActionResult> ExportOverallStats([FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var (from, to) = Validate(fromDate, toDate);

            var stats = await _memoService.GetStatsAsync(from, to);

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