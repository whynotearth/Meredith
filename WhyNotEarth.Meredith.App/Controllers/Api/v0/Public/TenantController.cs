using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Tenant;
using WhyNotEarth.Meredith.Data.Entity;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [ApiVersion("0")]
    [Route("api/v0/companies/{companySlug}/tenants")]
    [ProducesErrorResponseType(typeof(void))]
    public class TenantController : ControllerBase
    {
        private readonly MeredithDbContext _meredithDbContext;

        public TenantController(MeredithDbContext meredithDbContext)
        {
            _meredithDbContext = meredithDbContext;
        }

        [HttpGet("")]
        public async Task<ActionResult<List<TenantResult>>> List(string companySlug)
        {
            var tenants = await _meredithDbContext.Tenants
                .Include(item => item.Company)
                .Include(item => item.Logo)
                .Where(s => s.Company.Slug.ToLower() == companySlug.ToLower())
                .ToListAsync();


            return Ok(tenants.Select(item => new TenantResult(item)).ToList());
        }
    }
}