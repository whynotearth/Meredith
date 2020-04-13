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
    [Route("api/v0/products")]
    [ProducesErrorResponseType(typeof(void))]
    public class ProductsController : ControllerBase
    {
        private readonly MeredithDbContext _dbContext;

        public ProductsController(MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{tenantId}/categories")]
        public async Task<ActionResult<List<CategoryResult>>> Categories(int tenantId)
        {
            var productCategories = await _dbContext.Categories.OfType<ProductCategory>()
                .Where(item => item.TenantId == tenantId).ToListAsync();

            return Ok(productCategories.Select(item => new CategoryResult(item)).ToList());
        }

        [HttpGet("{tenantId}/{categoryId}")]
        public async Task<ActionResult<ProductResult>> Products(int tenantId, int categoryId)
        {
            var products = await _dbContext.Products.Where(item => item.Id == tenantId && item.CategoryId == categoryId)
                .ToListAsync();

            return Ok(products.Select(item => new ProductResult(item)).ToList());
        }
    }
}