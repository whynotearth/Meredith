using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Tenant;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Models;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Tenant
{
    [ApiVersion("0")]
    [Route("api/v0/companies/{companySlug}/tenants")]
    [ProducesErrorResponseType(typeof(void))]
    public class TenantController : BaseController
    {
        private readonly TenantService _tenantService;
        private readonly UserManager _userManager;

        public TenantController(UserManager userManager, TenantService tenantService)
        {
            _userManager = userManager;
            _tenantService = tenantService;
        }

        [Authorize]
        [Returns201]
        [Returns401]
        [Returns404]
        [HttpPost("")]
        public async Task<CreateObjectResult> Create(string companySlug, TenantModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            var tenantSlug = await _tenantService.CreateAsync(companySlug, model, user);

            return Created(tenantSlug);
        }

        [HttpGet("")]
        public async Task<ActionResult<List<TenantResult>>> List(string companySlug)
        {
            var tenants = await _tenantService.ListAsync(companySlug);

            return Ok(tenants.Select(item => new TenantResult(item)).ToList());
        }

        [Authorize]
        [HttpGet("mytenants")]
        public async Task<ActionResult<List<string>>> GetAllTenantsByUser(string companySlug, string? tenantSlug)
        {
            var user = await _userManager.GetUserAsync(User);
            var tenants = await _tenantService.GetAllTenantByUser(user, companySlug, tenantSlug);

            return Ok(tenants);
        }
    }
}