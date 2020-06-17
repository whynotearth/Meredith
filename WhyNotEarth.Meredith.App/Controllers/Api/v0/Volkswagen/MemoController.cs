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
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Volkswagen;
using WhyNotEarth.Meredith.Volkswagen.Models;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    [Returns401]
    [Returns403]
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

        [Returns201]
        [HttpPost("")]
        public async Task<StatusCodeResult> Create(MemoModel model)
        {
            await _memoService.CreateAsync(model);

            return new StatusCodeResult(StatusCodes.Status201Created);
        }

        [Returns200]
        [HttpGet("stats")]
        public async Task<ActionResult<List<MemoStatResult>>> Stats()
        {
            var stats = await _memoService.GetStatsAsync();

            return Ok(stats.Select(item => new MemoStatResult(item)));
        }

        [Returns200]
        [Returns404]
        [HttpGet("{memoId}/stats")]
        public async Task<ActionResult<List<MemoStatDetailResult>>> DetailStats(int memoId)
        {
            var memoInfo = await _memoService.GetStatsAsync(memoId);
            var result = new MemoStatDetailResult(memoInfo);

            var detailStats = await _emailRecipientService.GetMemoDetailStatsAsync(memoId);

            result.NotOpened.AddRange(detailStats.NotOpenedList.Select(item => new EmailRecipientResult(item)));

            result.Opened.AddRange(detailStats.OpenedList.Select(item => new EmailRecipientResult(item)));

            return Ok(result);
        }

        [Returns200]
        [HttpGet("overallstats")]
        public async Task<ActionResult<List<MemoStatResult>>> OverallStats([FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            var stats = await _memoService.GetStatsAsync(fromDate.Date, toDate.Date);

            return Ok(new MemoOverAllStatsResult(stats));
        }

        [Returns200]
        [HttpGet("exportuserstats")]
        public async Task<IActionResult> ExportOverallUserStats([FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            var stats = await _memoService.GetUserStatsAsync(fromDate.Date, toDate.Date);

            return await Csv(stats, _environment.IsDevelopment());
        }

        [Returns200]
        [HttpGet("exportopenstats")]
        public async Task<IActionResult> ExportOverallOpenStats([FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            var stats = await _memoService.GetOpenStatsAsync(fromDate.Date, toDate.Date);

            return await Csv(stats, _environment.IsDevelopment());
        }

        [Returns200]
        [HttpGet("exportclickstats")]
        public async Task<IActionResult> ExportOverallClickStats([FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            var stats = await _memoService.GetClickStatsAsync(fromDate.Date, toDate.Date);

            return await Csv(stats, _environment.IsDevelopment());
        }
    }
}