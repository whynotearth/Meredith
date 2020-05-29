using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.Shop;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Models;
using WhyNotEarth.Meredith.Shop;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Shop
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [Route("api/v0/shop/clients")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageTenant)]
    public class ClientController : BaseController
    {
        private readonly ClientService _clientService;
        private readonly TenantService _tenantService;
        private readonly IUserManager _userManager;

        public ClientController(ClientService clientService, TenantService tenantService, IUserManager userManager)
        {
            _clientService = clientService;
            _tenantService = tenantService;
            _userManager = userManager;
        }

        [Returns201]
        [Returns400]
        [HttpPost("")]
        public async Task<CreateResult> Create(ClientModel model)
        {
            var tenant = await GetCurrentTenantAsync();

            await _clientService.CreateAsync(model, tenant);

            return Created();
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<ClientResult>>> List()
        {
            var tenant = await GetCurrentTenantAsync();

            var clients = await _clientService.ListAsync(tenant);

            var list = clients.OrderBy(item => item.User.Name.ToUpper()).GroupBy(item => item.User.Name.ToUpper()[0]).ToList();

            return Ok(list.Select(item => new ClientListResult(item.Key, item)).ToList());
        }

        private async Task<Data.Entity.Models.Modules.Shop.Tenant> GetCurrentTenantAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var tenant = await _tenantService.GetTenant(user);

            if (tenant is null)
            {
                throw new ForbiddenException("You don't own any tenants");
            }

            return tenant;
        } 
    }
}