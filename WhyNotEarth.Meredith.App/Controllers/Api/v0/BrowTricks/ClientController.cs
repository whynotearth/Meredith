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
        [HttpPost("")]
        [Authorize(Policy = Policies.ManageTenant)]
        public async Task<CreateResult> Create(string tenantSlug, ClientModel model)
        {
            var user = await _userService.GetUserAsync(User);

            await _clientService.CreateAsync(tenantSlug, model, user);

            return Created();
        }
    }
}