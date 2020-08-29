using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Identity;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.BrowTricks
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [ProducesErrorResponseType(typeof(void))]
    [Route("api/v0/browtricks/tenants/{tenantSlug}/clients/{clientId}/pmu")]
    public class PmuController : BaseController
    {
        private readonly IPmuService _pmuService;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;

        public PmuController(IPmuService pmuService, IUserService userService, IWebHostEnvironment environment)
        {
            _pmuService = pmuService;
            _userService = userService;
            _environment = environment;
        }

        [Authorize]
        [Returns200]
        [Returns404]
        [HttpGet("")]
        public async Task<IActionResult> Get(int clientId)
        {
            var user = await GetCurrentUserAsync(_userService);

            var data = await _pmuService.GetPdfAsync(clientId, user);

            return Based64Pdf(data, _environment);
        }

        [Authorize]
        [Returns204]
        [HttpPost("")]
        public async Task<NoContentResult> Sign(int clientId)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _pmuService.SignAsync(clientId, user);

            return NoContent();
        }

        [Returns200]
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