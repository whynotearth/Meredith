namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using WhyNotEarth.Meredith.Data.Entity;

    [ApiVersion("0")]
    [Route("/api/v0/companies")]
    [EnableCors]
    public class CompanyController : Controller
    {
        private MeredithDbContext MeredithDbContext { get; }

        public CompanyController(MeredithDbContext meredithDbContext)
        {
            MeredithDbContext = meredithDbContext;
        }

        [Route("{companyId}/stripe/keys/publishable")]
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