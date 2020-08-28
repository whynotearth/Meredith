using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Identity;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.BrowTricks
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageTenant)]
    [Route("api/v0/browtricks/tenants/{tenantSlug}")]
    public class BrowtricksTenantController : BaseController
    {
        private readonly IBrowTricksTenantService _browTricksTenantService;
        private readonly IUserService _userService;

        public BrowtricksTenantController(IBrowTricksTenantService browTricksTenantService, IUserService userService)
        {
            _browTricksTenantService = browTricksTenantService;
            _userService = userService;
        }

        [Returns200]
        [Returns404]
        [HttpGet("")]
        public async Task<ActionResult<BrowtricksTenantResult>> Get(string tenantSlug)
        {
            var user = await GetCurrentUserAsync(_userService);

            var images = await _browTricksTenantService.GetAllImages(tenantSlug, user);
            var videos = await _browTricksTenantService.GetAllVideos(tenantSlug, user);

            return Ok(new BrowtricksTenantResult(images, videos));
        }
    }
}