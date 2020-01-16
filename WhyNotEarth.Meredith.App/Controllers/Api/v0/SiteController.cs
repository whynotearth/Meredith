namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using WhyNotEarth.Meredith.Data.Entity;

    [ApiVersion("0")]
    [Route("/api/v0/sites")]
    [EnableCors]
    public class SiteController : Controller
    {
        private MeredithDbContext MeredithDbContext { get; }

        public SiteController(MeredithDbContext meredithDbContext)
        {
            MeredithDbContext = meredithDbContext;
        }

        [HttpGet]
        [Route("{domain}")]
        public async Task<IActionResult> Get(string domain)
        {
            var site = await MeredithDbContext.Sites
                .Include(s => s.Company)
                .FirstOrDefaultAsync(s => s.Domain == domain);
            return Ok(new
            {
                site.Domain,
                Company = new
                {
                    site.Company.Id,
                    site.Company.Name,
                    site.Company.Slug
                }
            });
        }
    }
}