using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.App.Models.Api.v0.Subscription;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Profile;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Platform.Subscriptions;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Tenant
{
    [Authorize]
    [Returns401]
    [ApiVersion("0")]
    [Route("/api/v0/tenant/{tenantSlug}/subscriptions")]
    [ProducesErrorResponseType(typeof(void))]
    public class SubscriptionController : TenantControllerBase
    {
        private readonly CustomerService _customerService;

        private readonly SubscriptionService _subscriptionService;

        public SubscriptionController(
            CustomerService customerService,
            SubscriptionService subscriptionService,
            UserManager userManager,
            IDbContext dbContext) : base(dbContext, userManager)
        {
            _customerService = customerService;
            _subscriptionService = subscriptionService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string tenantSlug)
        {
            var tenant = await GetUserOwnedBySlug(tenantSlug);
            if (tenant == null)
            {
                return NotFound($"Tenant slug '{tenantSlug}' not found");
            }

            var subscription = await _dbContext.PlatformSubscriptions
                .Where(s => s.Customer.TenantId == tenant.Id && s.Status == Meredith.Public.Subscription.SubscriptionStatuses.Active)
                .Select(s => new
                {
                    s.Id,
                    s.Status,
                    Plan = s.Plan == null ? null : new
                    {
                        s.Plan.Id,
                        s.Plan.Name,
                        s.Plan.Price
                    },
                    Card = s.Card == null ? null : new
                    {
                        s.Card.Id,
                        s.Card.Brand,
                        s.Card.Last4,
                        s.Card.ExpirationMonth,
                        s.Card.ExpirationYear
                    }
                })
                .ToListAsync();
            return Ok(subscription);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string tenantSlug, [FromBody] CreateModel model)
        {
            try
            {
                var tenant = await GetUserOwnedBySlug(tenantSlug);
                if (tenant == null)
                {
                    return NotFound($"Tenant slug '{tenantSlug}' not found");
                }

                var companyId = await _dbContext.PlatformPlans
                    .Where(p => p.Id == model.PlanId)
                    .Select(p => p.Platform.CompanyId)
                    .FirstOrDefaultAsync();
                var customer = await _dbContext.PlatformCustomers
                    .Where(c => c.TenantId == tenant.Id)
                    .FirstOrDefaultAsync()
                    ?? await _customerService.AddCustomerAsync(tenant.Id, companyId);
                var subscription = await _subscriptionService.StartSubscriptionAsync(customer.Id, model.PlanId, model.CardId, model.CouponCode);
                return Ok();
            }
            catch (global::Stripe.StripeException exception) when (exception.Message.Contains("No such coupon"))
            {
                return BadRequest(new
                {
                    Message = "Invalid coupon code"
                });
            }
        }

        [HttpPost("changepaymentmethod")]
        public async Task<IActionResult> ChangePaymentMethod(string tenantSlug, [FromBody] ChangePaymentMethodModel model)
        {
            var tenant = await GetUserOwnedBySlug(tenantSlug);
            if (tenant == null)
            {
                return NotFound($"Tenant slug '{tenantSlug}' not found");
            }

            var subscriptionId = await GetSubscriptionIdFromTenantId(tenant.Id);
            if (subscriptionId == 0)
            {
                return NotFound("No active subscription found");
            }

            await _subscriptionService.ChangeSubscriptionCardAsync(subscriptionId, model.CardId);
            return Ok();
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel(string tenantSlug)
        {
            var tenant = await GetUserOwnedBySlug(tenantSlug);
            if (tenant == null)
            {
                return NotFound($"Tenant slug '{tenantSlug}' not found");
            }

            var subscriptionId = await GetSubscriptionIdFromTenantId(tenant.Id);
            if (subscriptionId == 0)
            {
                return NotFound("No active subscription found");
            }

            await _subscriptionService.CancelSubscriptionAsync(subscriptionId);
            return Ok();
        }

        [HttpGet("payments")]
        public async Task<ActionResult<List<PaymentModel>>> GetPayments(string tenantSlug)
        {
            var tenant = await GetUserOwnedBySlug(tenantSlug);
            if (tenant == null)
            {
                return NotFound($"Tenant slug '{tenantSlug}' not found");
            }

            var customerId = await _dbContext.PlatformCustomers
                .Where(pc => pc.TenantId == tenant.Id)
                .Select(pc => pc.Id)
                .FirstOrDefaultAsync();
            var transactions = await _customerService.GetTransactions(customerId);
            return Ok(transactions
                .OrderByDescending(t => t.Date)
                .Select(t => new PaymentModel
                {
                    PaymentDate = t.Date,
                    Total = t.Amount,
                    PaymentMethod = t.PaymentMethod,
                    TransactionId = t.TransactionId,
                    StatementLink = t.StatementLink
                })
                .ToList());
        }

        private async Task<int> GetSubscriptionIdFromTenantId(int tenantId) => await _dbContext.PlatformSubscriptions
                .Where(s => s.Customer.TenantId == tenantId && s.Status == Meredith.Public.Subscription.SubscriptionStatuses.Active)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();
    }
}