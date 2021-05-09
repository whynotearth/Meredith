using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Models.Api.v0.User;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.BrowTricks.Services;
using WhyNotEarth.Meredith.Identity;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.BrowTricks
{
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

        [Returns404]
        [HttpPost("")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<CreateResult> Create(string tenantSlug, ClientModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _clientService.CreateAsync(tenantSlug, model, user);

            return Created();
        }

        [Returns404]
        [HttpPut("{clientId}")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<NoContentResult> Edit(int clientId, ClientModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _clientService.EditAsync(clientId, model, user);

            return NoContent();
        }

        [HttpGet("")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<ClientListResult> List(string tenantSlug, [FromQuery] UserSearchModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            var (clients, total) = await _clientService.GetListAsync(tenantSlug, user, model.Page, model.Query);

            return new ClientListResult()
            {
                Total = total,
                CurrentPage = model.Page,
                PerPage = 100,
                Records = clients.Select(item => new ClientListResult.ClientResult(item)).ToList()
            };

        }

        [Authorize]
        [Returns404]
        [HttpGet("{clientId}")]
        public async Task<ClientGetResult> Get(int clientId)
        {
            var user = await GetCurrentUserAsync(_userService);

            var client = await _clientService.GetAsync(clientId, user);
            var signatureUrls = await _formSignatureService.GetSignatureUrlsAsync(client);

            return new ClientGetResult(client, signatureUrls);
        }

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