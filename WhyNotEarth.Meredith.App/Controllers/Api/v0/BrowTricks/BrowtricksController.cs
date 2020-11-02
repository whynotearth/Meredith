using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks;
using WhyNotEarth.Meredith.BrowTricks.Services;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.BrowTricks
{
    [ApiVersion("0")]
    [Route("api/v0/browtricks/")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageTenant)]
    public class BrowtricksController : BaseController
    {
        private readonly IBrowTricksService _browTricksService;
        private readonly IUserService _userService;
        private readonly TenantService _tenantService;

        public BrowtricksController(IBrowTricksService browTricksService, IUserService userService, TenantService tenantService)
        {
            _browTricksService = browTricksService;
            _userService = userService;
            _tenantService = tenantService;
        }

        [Returns200]
        [HttpGet("profile")]
        public async Task<ActionResult<BrowtricksProfileResult>> GetProfile()
        {
            var user = await GetCurrentUserAsync(_userService);

            var images = await _browTricksService.GetAllImages(user);
            var videos = await _browTricksService.GetAllVideos(user);

            return new BrowtricksProfileResult(user, images, videos);
        }

        [Returns200]
        [Returns404]
        [HttpGet("tenants/{tenantSlug}")]
        public async Task<ActionResult<BrowtricksTenantResult>> GetTenant(string tenantSlug)
        {
            var user = await GetCurrentUserAsync(_userService);

            var tenant = await _tenantService.GeAsync(tenantSlug);
            var images = await _browTricksService.GetAllImages(tenantSlug, user);
            var videos = await _browTricksService.GetAllVideos(tenantSlug, user);

            return new BrowtricksTenantResult(tenant, images, videos);
        }
    }
}