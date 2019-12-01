namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using WhyNotEarth.Meredith.App.Models.Api.v0.Company;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.Exceptions;
    using WhyNotEarth.Meredith.Public;

    [ApiVersion("0")]
    [Route("/api/v0/companies")]
    [EnableCors]
    public class CompanyController : Controller
    {
        private MeredithDbContext MeredithDbContext { get; }
        private CompanyService CompanyService { get; }

        public CompanyController(MeredithDbContext meredithDbContext, CompanyService publicService)
        {
            CompanyService = publicService;
            MeredithDbContext = meredithDbContext;
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> Create(CompanyModel company)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var newCompany = await CompanyService.CreateCompanyAsync(company.Name, company.Slug);
            return Ok(new { CompanyId = newCompany.Id });
        }

        [Route("{companyId}/stripe/keys/publishable")]
        [HttpGet]
        public async Task<IActionResult> StripePublishableKey(int companyId)
        {
            var key = await MeredithDbContext.StripeAccounts
                .Where(s => s.CompanyId == companyId)
                .Select(s => s.StripePublishableKey)
                .FirstOrDefaultAsync();

            if (key == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                key
            });
        }
    }
}