using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Identity;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.BrowTricks
{
    [ApiVersion("0")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageTenant)]
    [Route("api/v0/browtricks/tenants/{tenantSlug}/pmu/questions")]
    public class PmuQuestionController : BaseController
    {
        private readonly IPmuQuestionService _pmuQuestionService;
        private readonly IUserService _userService;

        public PmuQuestionController(IPmuQuestionService pmuQuestionService, IUserService userService)
        {
            _pmuQuestionService = pmuQuestionService;
            _userService = userService;
        }

        [Returns201]
        [Returns401]
        [Returns403]
        [Returns404]
        [HttpPost("")]
        public async Task<CreateResult> Create(string tenantSlug, PmuQuestionModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _pmuQuestionService.CreateAsync(tenantSlug, model, user);

            return Created();
        }

        [Returns204]
        [Returns401]
        [Returns403]
        [Returns404]
        [HttpPut("{questionId}")]
        public async Task<NoContentResult> Edit(string tenantSlug, int questionId, PmuQuestionModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _pmuQuestionService.EditAsync(tenantSlug, questionId, model, user);

            return NoContent();
        }

        [Returns204]
        [Returns401]
        [Returns403]
        [Returns404]
        [HttpPost("{questionId}")]
        public async Task<NoContentResult> Delete(string tenantSlug, int questionId)
        {
            var user = await _userService.GetUserAsync(User);

            await _pmuQuestionService.DeleteAsync(tenantSlug, questionId, user);

            return NoContent();
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<PmuQuestionResult>>> List(string tenantSlug)
        {
            var questions = await _pmuQuestionService.ListAsync(tenantSlug);

            return Ok(questions.Select(item => new PmuQuestionResult(item)));
        }
    }
}