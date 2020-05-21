using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [Route("api/v0/volkswagen/memos")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageVolkswagen)]
    public class MemoController : ControllerBase
    {
        private readonly EmailRecipientService _emailRecipientService;
        private readonly MemoService _memoService;

        public MemoController(MemoService memoService, EmailRecipientService emailRecipientService)
        {
            _memoService = memoService;
            _emailRecipientService = emailRecipientService;
        }

        [Returns201]
        [HttpPost("")]
        public async Task<StatusCodeResult> Create(MemoModel model)
        {
            await _memoService.CreateAsync(model.DistributionGroups!, model.Subject!, model.Date!, model.To!,
                model.Description!);

            return new StatusCodeResult(StatusCodes.Status201Created);
        }

        [Returns200]
        // TODO: Add stats to route
        [HttpGet("")]
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
    }
}