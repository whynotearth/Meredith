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
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [ProducesErrorResponseType(typeof(void))]
    [Route("api/v0/browtricks/tenants/{tenantSlug}/disclosures")]
    public class DisclosuresController : BaseController
    {
        private readonly IDisclosureService _disclosureService;
        private readonly IUserService _userService;

        public DisclosuresController(IDisclosureService disclosureService, IUserService userService)
        {
            _disclosureService = disclosureService;
            _userService = userService;
        }

        [Returns204]
        [HttpPost("")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<NoContentResult> Create(string tenantSlug, DisclosureModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _disclosureService.CreateAsync(tenantSlug, model, user);

            return NoContent();
        }

        [Authorize]
        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<DisclosureResult>>> List(string tenantSlug)
        {
            var user = await GetCurrentUserAsync(_userService);

            var disclosures = await _disclosureService.ListAsync(tenantSlug, user);

            return Ok(disclosures.Select(item => new DisclosureResult(item)));
        }
    }
}