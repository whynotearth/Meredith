using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Identity;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.BrowTricks
{
    [ApiVersion("0")]
    [ProducesErrorResponseType(typeof(void))]
    [Route("api/v0/browtricks/tenants/{tenantSlug}/clients/{clientId}/pmu")]
    public class PmuController : BaseController
    {
        private readonly IPmuService _pmuService;
        private readonly IUserService _userService;

        public PmuController(IPmuService pmuService, IUserService userService)
        {
            _pmuService = pmuService;
            _userService = userService;
        }

        [Authorize]
        [Returns200]
        [Returns404]
        [Returns401]
        [Returns403]
        [HttpPost("")]
        public async Task<ActionResult<string>> Pmu(int clientId, ClientPmuModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            var url = await _pmuService.SetAsync(clientId, model, user);

            return Ok(url);
        }

        [Authorize]
        [Returns204]
        [Returns401]
        [Returns403]
        [HttpPost("signed")]
        public async Task<NoContentResult> Signed(int clientId)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _pmuService.SetSignedAsync(clientId, user);

            return NoContent();
        }

        [Returns200]
        [Returns401]
        [Returns403]
        [Returns404]
        [HttpPost("notify")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<ActionResult<string>> PmuSmsNotification(int clientId, [FromQuery] string callbackUrl)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _pmuService.SendConsentNotificationAsync(clientId, user, callbackUrl);

            return Ok();
        }
    }
}