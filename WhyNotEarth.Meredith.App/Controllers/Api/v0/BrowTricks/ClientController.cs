using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Authorize(Policy = Policies.ManageTenant)]
    [Route("api/v0/browtricks/tenant/{tenantSlug}/clients")]
    public class ClientController : BaseController
    {
        private readonly ClientService _clientService;
        private readonly IUserService _userService;

        public ClientController(ClientService clientService, IUserService userService)
        {
            _clientService = clientService;
            _userService = userService;
        }

        [Returns201]
        [Returns404]
        [HttpPost("")]
        public async Task<CreateResult> Create(string tenantSlug, ClientModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientService.CreateAsync(tenantSlug, model, user);

            return Created();
        }

        [Returns204]
        [Returns404]
        [HttpPut("{clientId}")]
        public async Task<NoContentResult> Edit(int clientId, ClientModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientService.EditAsync(clientId, model, user);

            return NoContent();
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<ClientResult>>> List(string tenantSlug)
        {
            var user = await _userService.GetUserAsync(User);

            var clients = await _clientService.GetListAsync(tenantSlug, user);

            return Ok(clients.Select(item => new ClientResult(item)));
        }

        [Returns204]
        [Returns404]
        [HttpPost("{clientId}/archive")]
        public async Task<NoContentResult> Archive(int clientId)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientService.ArchiveAsync(clientId, user);

            return NoContent();
        }

        [Returns204]
        [Returns404]
        [HttpPost("{clientId}/pmu")]
        public async Task<NoContentResult> Pmu(int clientId, ClientPmuModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientService.SetPmuAsync(clientId, model, user);

            return NoContent();
        }
    }
}