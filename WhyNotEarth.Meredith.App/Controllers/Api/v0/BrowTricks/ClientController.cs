using System.Collections.Generic;
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
    [ApiVersion("0")]
    [ProducesErrorResponseType(typeof(void))]
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
        [Returns401]
        [Returns403]
        [Returns404]
        [HttpPost("")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<CreateResult> Create(string tenantSlug, ClientModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientService.CreateAsync(tenantSlug, model, user);

            return Created();
        }

        [Returns204]
        [Returns401]
        [Returns403]
        [Returns404]
        [HttpPost("{clientId}")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<NoContentResult> Edit(int clientId, ClientModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientService.EditAsync(clientId, model, user);

            return NoContent();
        }

        [Returns200]
        [Returns401]
        [Returns403]
        [HttpGet("")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<ActionResult<List<ClientResult>>> List(string tenantSlug)
        {
            var user = await _userService.GetUserAsync(User);

            var clients = await _clientService.GetListAsync(tenantSlug, user);

            return Ok(clients);
        }

        [Returns204]
        [Returns401]
        [Returns403]
        [Returns404]
        [HttpPost("{clientId}/archive")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<NoContentResult> Archive(int clientId)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientService.ArchiveAsync(clientId, user);

            return NoContent();
        }

        [Returns204]
        [Returns401]
        [Returns403]
        [Returns404]
        [HttpDelete("{clientId}")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<NoContentResult> Delete(int clientId)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientService.DeleteAsync(clientId, user);

            return NoContent();
        }

        [Returns204]
        [Returns401]
        [Returns403]
        [Returns404]
        [HttpPost("{clientId}/pmu")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<NoContentResult> Pmu(int clientId, ClientPmuModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientService.SetPmuAsync(clientId, model, user);

            return NoContent();
        }
    }
}