using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen;
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
        private readonly MemoRecipientService _memoRecipientService;
        private readonly MemoService _memoService;

        public MemoController(MemoService memoService, MemoRecipientService memoRecipientService)
        {
            _memoService = memoService;
            _memoRecipientService = memoRecipientService;
        }

        [Returns201]
        [HttpPost("")]
        public async Task<StatusCodeResult> Create(MemoModel model)
        {
            await _memoService.CreateAsync(model.DistributionGroup, model.Subject, model.Date, model.To,
                model.Description);

            return new StatusCodeResult(StatusCodes.Status201Created);
        }

        [HttpGet("")]
        public async Task<ActionResult<List<MemoResult>>> List()
        {
            var memoInfos = await _memoService.GetListAsync();

            return Ok(memoInfos.Select(item => new MemoResult(item)).ToList());
        }

        [HttpGet("{memoId}/stats")]
        public async Task<ActionResult<List<MemoStatsResult>>> Details(int memoId)
        {
            var memoInfo = await _memoService.Get(memoId);
            var result = new MemoStatsResult(memoInfo);

            var memoStats = await _memoRecipientService.GetMemoStats(memoId);

            result.NotOpened.AddRange(memoStats.NotOpenedList.Select(item => new MemoRecipientResult(item)));

            result.Opened.AddRange(memoStats.OpenedList.Select(item => new MemoRecipientResult(item)));

            return Ok(result);
        }
    }
}