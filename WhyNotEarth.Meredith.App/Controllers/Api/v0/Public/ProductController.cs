using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Products;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [ApiVersion("0")]
    [Route("api/v0/tenants/{tenantSlug}")]
    [ProducesErrorResponseType(typeof(void))]
    public class ProductsController : ControllerBase
    {
        private readonly MeredithDbContext _dbContext;

        public ProductsController(MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryResult>>> Categories(string tenantSlug)
        {
            var productCategories = await _dbContext.Categories.OfType<ProductCategory>()
                .Include(item => item.Tenant)
                .Include(item => item.Image)
                .Where(item => item.Tenant.Slug.ToLower() == tenantSlug.ToLower())
                .ToListAsync();

            return Ok(productCategories.Select(item => new CategoryResult(item)).ToList());
        }

        [HttpGet("categories/{categorySlug}/products")]
        public async Task<ActionResult<ProductResult>> Products(string tenantSlug, string categorySlug)
        {
            var products = await _dbContext.Products
                .Include(item => item.Tenant)
                .Include(item => item.Category)
                .Include(item => item.Images)
                .Where(item =>
                    item.Tenant.Slug.ToLower() == tenantSlug.ToLower() &&
                    item.Category.Slug.ToLower() == categorySlug.ToLower())
                .ToListAsync();

            return Ok(products.Select(item => new ProductResult(item)).ToList());
        }
    }
}