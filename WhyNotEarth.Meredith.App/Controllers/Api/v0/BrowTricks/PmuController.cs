﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.BrowTricks.Services;
using WhyNotEarth.Meredith.Identity;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.BrowTricks
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [ProducesErrorResponseType(typeof(void))]
    [Route("api/v0/browtricks/tenants/{tenantSlug}/pmu")]
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
        public async Task<IActionResult> Get(string tenantSlug)
        {
            var user = await GetCurrentUserAsync(_userService);

            var data = await _pmuService.GetPngAsync(tenantSlug, user);

            return Based64Png(data, _environment);
        }

        [Authorize]
        [Returns200]
        [Returns404]
        [HttpGet("{clientId}")]
        public async Task<IActionResult> GetByClient(int clientId)
        {
            var user = await GetCurrentUserAsync(_userService);

            var data = await _pmuService.GetPngAsync(clientId, user);

            return Based64Png(data, _environment);
        }

        [Authorize]
        [Returns204]
        [HttpPost("{clientId}")]
        public async Task<NoContentResult> Sign(int clientId, PmuSignModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _pmuService.SignAsync(clientId, model, user);

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