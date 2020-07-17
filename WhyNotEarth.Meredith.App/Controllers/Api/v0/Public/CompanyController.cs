using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Stripe.Data;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [ApiVersion("0")]
    [Route("api/v0/companies")]
    [ProducesErrorResponseType(typeof(void))]
    public class CompanyController : ControllerBase
    {
        private readonly MeredithDbContext _meredithDbContext;
        private readonly StripeOptions _stripeOptions;

        public CompanyController(MeredithDbContext meredithDbContext, IOptions<StripeOptions> stripeOptions)
        {
            _meredithDbContext = meredithDbContext;
            _stripeOptions = stripeOptions.Value;
        }

        [HttpGet("{companyId}/stripe/keys/publishable")]
        public async Task<IActionResult> StripePublishableKey(int companyId)
        {
            var accountId = await _meredithDbContext.StripeAccounts
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
                key = _stripeOptions.PublishableKey
            });
        }
    }
}