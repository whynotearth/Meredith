using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.BrowTricks.Services;
using WhyNotEarth.Meredith.Identity;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.BrowTricks
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [ProducesErrorResponseType(typeof(void))]
    [Route("api/v0/browtricks/tenants/{tenantSlug}/clients")]
    public class ClientController : BaseController
    {
        private readonly IClientService _clientService;
        private readonly IFormSignatureService _formSignatureService;
        private readonly IUserService _userService;

        public ClientController(IClientService clientService, IUserService userService,
            IFormSignatureService formSignatureService)
        {
            _clientService = clientService;
            _userService = userService;
            _formSignatureService = formSignatureService;
        }

        [Returns201]
        [Returns404]
        [HttpPost("")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<CreateResult> Create(string tenantSlug, ClientModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _clientService.CreateAsync(tenantSlug, model, user);

            return Created();
        }

        [Returns204]
        [Returns404]
        [HttpPut("{clientId}")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<NoContentResult> Edit(int clientId, ClientModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _clientService.EditAsync(clientId, model, user);

            return NoContent();
        }

        [Returns200]
        [HttpGet("")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<ActionResult<List<ClientListResult>>> List(string tenantSlug)
        {
            var user = await GetCurrentUserAsync(_userService);

            var clients = await _clientService.GetListAsync(tenantSlug, user);

            return clients.Select(item => new ClientListResult(item)).ToList();
        }

        [Authorize]
        [Returns200]
        [Returns404]
        [HttpGet("{clientId}")]
        public async Task<ActionResult<ClientGetResult>> Get(int clientId)
        {
            var user = await GetCurrentUserAsync(_userService);

            var client = await _clientService.GetAsync(clientId, user);
            var signatureUrls = await _formSignatureService.GetSignatureUrlsAsync(client);

            return new ClientGetResult(client, signatureUrls);
        }

        [Returns204]
        [Returns404]
        [HttpPost("{clientId}/archive")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<NoContentResult> Archive(int clientId)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _clientService.ArchiveAsync(clientId, user);

            return NoContent();
        }
    }
}