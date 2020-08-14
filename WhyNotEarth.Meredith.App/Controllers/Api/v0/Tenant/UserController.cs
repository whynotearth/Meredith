using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.Tenant;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Tenant
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageTenant)]
    [Route("/api/v0/companies/{companySlug}/tenants/users")]
    public class UserController : BaseController
    {
        private readonly TenantService _tenantService;
        private readonly IUserService _userService;

        public UserController(TenantService tenantService, IUserService userService)
        {
            _tenantService = tenantService;
            _userService = userService;
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<UserResult>>> List()
        {
            var tenant = await GetCurrentTenantAsync();

            var clients = await _userService.ListAsync(tenant);

            var list = clients.OrderBy(item => item.LastName?.ToUpper())
                .ThenBy(item => item.FirstName)
                .GroupBy(item => item.LastName?.ToUpper()[0]).ToList();

            return Ok(list.Select(item => new UserListResult(item.Key, item)).ToList());
        }

        private async Task<Meredith.Public.Tenant> GetCurrentTenantAsync()
        {
            var user = await _userService.GetUserAsync(User);
            var tenant = await _tenantService.GetTenant(user);

            if (tenant is null)
            {
                throw new ForbiddenException("You don't own any tenants");
            }

            return tenant;
        }
    }
}