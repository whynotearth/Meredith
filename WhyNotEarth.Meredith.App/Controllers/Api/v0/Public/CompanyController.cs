using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Stripe.Data;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [ApiVersion("0")]
    [Route("api/v0/companies")]
    public class CompanyController : ControllerBase
    {
        public CompanyController(
            MeredithDbContext meredithDbContext,
            IOptions<StripeOptions> stripeOptions)
        {
            MeredithDbContext = meredithDbContext;
            StripeOptions = stripeOptions.Value;
        }

        private MeredithDbContext MeredithDbContext { get; }

        private StripeOptions StripeOptions { get; }

        [Route("{companyId}/stripe/keys/publishable")]
        [HttpGet]
        public async Task<IActionResult> StripePublishableKey(int companyId)
        {
            var accountId = await MeredithDbContext.StripeAccounts
                .Where(s => s.CompanyId == companyId)
                .Select(s => s.StripeUserId)
                .FirstOrDefaultAsync();

            if (accountId == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                accountId,
                key = StripeOptions.PublishableKey
            });
        }
    }
}