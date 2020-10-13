using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Tenant;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Identity.Models;
using WhyNotEarth.Meredith.Tenant;
using WhyNotEarth.Meredith.Tenant.Models;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Tenant
{
    [ApiVersion("0")]
    [Route("api/v0/companies/{companySlug}/tenants")]
    [ProducesErrorResponseType(typeof(void))]
    public class TenantController : BaseController
    {
        private readonly TenantService _tenantService;
        private readonly SeoSchemaService _seoSchemaService;
        private readonly UserManager _userManager;
        private readonly IUserService _userService;

        public TenantController(UserManager userManager, TenantService tenantService, SeoSchemaService seoSchemaService, IUserService userService)
        {
            _userManager = userManager;
            _tenantService = tenantService;
            _seoSchemaService = seoSchemaService;
            _userService = userService;
        }

        [Authorize]
        [Returns201]
        [Returns401]
        [Returns403]
        [HttpPost("")]
        public async Task<CreateObjectResult> Create(string companySlug, TenantCreateModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            var tenantSlug = await _tenantService.CreateAsync(companySlug, model, user);

            return Created(tenantSlug);
        }

        [Authorize]
        [Returns204]
        [Returns401]
        [Returns403]
        [HttpPost("{tenantSlug}/active")]
        public async Task<NoContentResult> SetActivity(string tenantSlug, TenantActivityModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            await _tenantService.SetActivityAsync(tenantSlug, model, user);

            return NoContent();
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

            var schema = _seoSchemaService.CreateTenantShopSchema(tenant);

            return Ok(new TenantResult(tenant, schema));
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
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            return Ok(tenant != null!);
        }

        [Returns200]
        [HttpGet("{tenantSlug}/address")]
        public async Task<ActionResult<AddressResult>> GetAddress(string tenantSlug)
        {
            var address = await _tenantService.GetAddressAsync(tenantSlug);

            return Ok(new AddressResult(address));
        }

        [Authorize]
        [Returns201]
        [Returns401]
        [Returns403]
        [HttpPost("{tenantSlug}/address")]
        public async Task<CreateResult> SetAddress(string tenantSlug, AddressModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            await _tenantService.SetAddressAsync(tenantSlug, model, user);

            return Created();
        }

        [Authorize]
        [Returns204]
        [Returns401]
        [Returns403]
        [HttpPatch("{tenantSlug}")]
        public async Task<NoContentResult> Edit(string tenantSlug, TenantEditModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            await _tenantService.EditAsync(tenantSlug, model, user);

            return NoContent();
        }
        
        [Authorize]
        [Returns204]
        [Returns400]
        [HttpPost("confirmphone")]
        public async Task<IActionResult> ConfirmTempPhoneNumber(ConfirmTempPhoneNumberModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var tenant = await _tenantService.CheckOwnerAsync(user, model.TenantSlug);
            
            var identityResult = await _userService.ConfirmPhoneNumberAsync(user, new ConfirmPhoneNumberModel() { Token = model.Token });

            // If change on user was successful, we can change the phone number onf the tenant.
            if (identityResult.Succeeded)
            {
                await _tenantService.ConfirmTempPhoneNumber(tenant.Slug, user);
            }

            return NoContentIdentityResult(identityResult);
        }
        
        [Authorize]
        [Returns204]
        [Returns404]
        [HttpPost("confirmphonetoken")]
        public async Task<NoContentResult> SendConfirmPhoneNumberToken(ConfirmPhoneNumberTokenModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            await _userService.SendConfirmPhoneNumberTokenAsync(user, model);

            return NoContent();
        }

        private IActionResult NoContentIdentityResult(IdentityResult identityResult)
        {
            if (identityResult.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(identityResult.Errors);
        }
    }
}