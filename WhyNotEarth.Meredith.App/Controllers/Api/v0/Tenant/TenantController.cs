using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Tenant;
using WhyNotEarth.Meredith.Exceptions;
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
        [Returns403]
        [HttpPost("")]
        public async Task<CreateObjectResult> Create(string companySlug, TenantModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            var tenantSlug = await _tenantService.CreateAsync(companySlug, model, user);

            return Created(tenantSlug);
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<TenantListResult>>> List(string companySlug)
        {
            var tenants = await _tenantService.ListAsync(companySlug);

            return Ok(tenants.Select(item => new TenantResult(item)).ToList());
        }

        [Authorize]
        [Returns200]
        [Returns401]
        [HttpGet("mytenants")]
        public async Task<ActionResult<List<TenantListResult>>> GetAllTenantsByUser(string companySlug)
        {
            var user = await _userManager.GetUserAsync(User);
            var tenants = await _tenantService.GetAllTenantsByUser(user, companySlug);

            return Ok(tenants.Select(item => new TenantResult(item)).ToList());
        }

        [Returns200]
        [Returns404]
        [HttpGet("{tenantSlug}")]
        public async Task<ActionResult<TenantResult>> Get(string tenantSlug)
        {
            var tenant = await _tenantService.GeAsync(tenantSlug);

            if (tenant is null)
            {
                throw new RecordNotFoundException($"Tenant {tenantSlug} not found");
            }

            return Ok(new TenantResult(tenant));
        }

        [Authorize]
        [Returns200]
        [Returns401]
        [HttpGet("owns/{tenantSlug}")]
        // I tried my best to not let this become a thing but my best simply wasn't good enough
        // Atharva asked for it and Paulchrisluke signed off on it
        public async Task<ActionResult<bool>> DoesUserOwnTenant(string tenantSlug)
        {
            var user = await _userManager.GetUserAsync(User);
            var isOwnsTheTenant = await _tenantService.IsOwnsTheTenant(user, tenantSlug);

            return Ok(isOwnsTheTenant);
        }
    }
}