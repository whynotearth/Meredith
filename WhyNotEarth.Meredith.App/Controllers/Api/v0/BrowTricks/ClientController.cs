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
using WhyNotEarth.Meredith.HelloSign;
using WhyNotEarth.Meredith.Identity;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.BrowTricks
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageTenant)]
    [Route("api/v0/browtricks/tenants/{tenantSlug}/clients")]
    public class ClientController : BaseController
    {
        private readonly IClientService _clientService;
        private readonly IHelloSignService _helloSignService;
        private readonly IUserService _userService;

        public ClientController(IClientService clientService, IUserService userService,
            IHelloSignService helloSignService)
        {
            _clientService = clientService;
            _userService = userService;
            _helloSignService = helloSignService;
        }

        [Returns201]
        [Returns404]
        [HttpPost("")]
        public async Task<CreateResult> Create(string tenantSlug, ClientModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _clientService.CreateAsync(tenantSlug, model, user);

            return Created();
        }

        [Returns204]
        [Returns404]
        [HttpPut("{clientId}")]
        public async Task<NoContentResult> Edit(int clientId, ClientModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _clientService.EditAsync(clientId, model, user);

            return NoContent();
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<ClientResult>>> List(string tenantSlug)
        {
            var user = await GetCurrentUserAsync(_userService);

            var clients = await _clientService.GetListAsync(tenantSlug, user);

            return Ok(clients.Select(item => new ClientResult(item)));
        }

        [Returns204]
        [Returns404]
        [HttpPost("{clientId}/archive")]
        public async Task<NoContentResult> Archive(int clientId)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _clientService.ArchiveAsync(clientId, user);

            return NoContent();
        }

        [Returns204]
        [Returns404]
        [HttpPost("{clientId}/pmu")]
        public async Task<NoContentResult> Pmu(int clientId, ClientPmuModel model)
        {
            var user = await GetCurrentUserAsync(_userService);

            await _clientService.SetPmuAsync(clientId, model, user);

            return NoContent();
        }

        [Returns200]
        [HttpPost("{clientId}/pmu/sign")]
        public async Task<ActionResult<string>> Sign(int clientId)
        {
            var user = await GetCurrentUserAsync(_userService);

            var url = await _helloSignService.GetSignatureRequestAsync(clientId, user);

            return Ok(url);
        }
    }
}