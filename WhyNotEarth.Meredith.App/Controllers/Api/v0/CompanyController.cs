namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using WhyNotEarth.Meredith.App.Models.Api.v0.Company;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.Public;
    using WhyNotEarth.Meredith.Stripe.Data;

    [ApiVersion("0")]
    [Route("/api/v0/companies")]
    public class CompanyController : ControllerBase
    {
        public CompanyController(
            MeredithDbContext meredithDbContext,
            CompanyService publicService,
            IOptions<StripeOptions> stripeOptions)
        {
            CompanyService = publicService;
            MeredithDbContext = meredithDbContext;
            StripeOptions = stripeOptions.Value;
        }

        private CompanyService CompanyService { get; }

        private MeredithDbContext MeredithDbContext { get; }

        private StripeOptions StripeOptions { get; }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> Create(CompanyModel company)
        {
            var newCompany = await CompanyService.CreateCompanyAsync(company.Name, company.Slug);
            return Ok(new { CompanyId = newCompany.Id });
        }

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