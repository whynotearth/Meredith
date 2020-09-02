using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Profile;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [Authorize]
    [Returns401]
    [ApiVersion("0")]
    [Route("/api/v0/subscriptions")]
    [ProducesErrorResponseType(typeof(void))]
    public class SubscriptionController : ControllerBase
    {
        private readonly IDbContext _dbContext;

        public SubscriptionController(
            IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("domain/{domain}")]
        public async Task<ActionResult<ProfileResult>> ByDomain(string domain)
        {
            var plans = await _dbContext
                .PlatformPlans
                .Where(p => p.Platform.Domain == domain)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                })
                .ToListAsync();
            return Ok(plans);
        }
    }
}