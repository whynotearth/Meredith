using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WhyNotEarth.Meredith.Stripe.Data;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [ApiVersion("0")]
    [Route("api/v0/companies")]
    [ProducesErrorResponseType(typeof(void))]
    public class CompanyController : ControllerBase
    {
        private readonly IDbContext _dbContext;
        private readonly StripeOptions _stripeOptions;

        public CompanyController(IDbContext IDbContext, IOptions<StripeOptions> stripeOptions)
        {
            _dbContext = IDbContext;
            _stripeOptions = stripeOptions.Value;
        }

        [HttpGet("{companyId}/stripe/keys/publishable")]
        public async Task<IActionResult> StripePublishableKey(int companyId)
        {
            var accountId = await _dbContext.StripeAccounts
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