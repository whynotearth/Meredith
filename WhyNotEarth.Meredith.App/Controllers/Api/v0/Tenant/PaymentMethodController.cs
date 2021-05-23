using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.App.Models.Api.v0.Subscription;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Platform.Subscriptions;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [Authorize]
    [Returns401]
    [ApiVersion("0")]
    [Route("/api/v0/tenant/{tenantSlug}/paymentmethods")]
    [ProducesErrorResponseType(typeof(void))]
    public class PaymentMethodController : TenantControllerBase
    {
        private readonly CustomerService _customerService;

        public PaymentMethodController(
            CustomerService customerService,
            UserManager userManager,
            IDbContext dbContext) : base(dbContext, userManager)
        {
            _customerService = customerService;
        }

        [HttpGet("payments")]
        public async Task<ActionResult<PaymentModel>> GetPayments(string tenantSlug)
        {
            var payments = new PaymentModel();
            return Ok(payments);
        }

        [HttpGet]
        public async Task<ActionResult<List<Meredith.Public.PaymentCard>>> GetPaymentMethods(string tenantSlug)
        {
            var paymentMethods = await _dbContext.PlatformCards
                .Where(pc => pc!.Customer!.Tenant.Slug == tenantSlug)
                .ToListAsync();
            return Ok(paymentMethods);
        }

        [Returns400]
        [Returns404]
        [HttpPost]
        public async Task<ActionResult<List<PaymentModel>>> AddPaymentMethod(string tenantSlug, [FromBody] CreatePaymentMethodModel model)
        {
            var tenant = await GetUserOwnedBySlug(tenantSlug);
            if (tenant == null)
            {
                return NotFound($"Tenant slug '{tenantSlug}' not found");
            }

            await _customerService.AddCardAsync(tenant.Id, model.Token);
            return Ok();
        }

        [Returns400]
        [Returns404]
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<PaymentModel>>> DeletePaymentMethod(string tenantSlug, int id)
        {
            var tenant = await GetUserOwnedBySlug(tenantSlug);
            if (tenant == null)
            {
                return NotFound($"Tenant slug '{tenantSlug}' not found");
            }

            if (!await _dbContext.PlatformCards.AnyAsync(c => c.Id == id && c!.Customer!.Tenant.Id == tenant.Id))
            {
                return NotFound($"Card with id {id} not found");
            }

            await _customerService.DeleteCardAsync(id);
            return Ok();
        }

        [Returns400]
        [Returns404]
        [HttpGet("stripe")]
        public async Task<ActionResult<List<PaymentModel>>> GetStripe(string tenantSlug)
        {
            var tenant = await GetUserOwnedBySlug(tenantSlug);
            if (tenant == null)
            {
                return NotFound($"Tenant slug '{tenantSlug}' not found");
            }

            var publishableKey = await _dbContext.PlatformCustomers
                .Where(c => c.TenantId == tenant.Id)
                .Select(c => c.Company!.StripeAccount!.StripePublishableKey)
                .FirstOrDefaultAsync();
            return Ok(new
            {
                PublishableKey = publishableKey
            });
        }


    }
}