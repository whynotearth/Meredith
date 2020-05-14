using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.Public
{
    public class ProductsService
    {
        private readonly MeredithDbContext _dbContext;

        public ProductsService(MeredithDbContext meredithDbContext)
        {
            _dbContext = meredithDbContext;
        }

        public async Task<List<Product>> GetProductListAsync(string tenantSlug, string categorySlug)
        {
            return await _dbContext.Products
                .Include(item => item.Tenant)
                .Include(item => item.Category)
                .Include(item => item.Images)
                .Where(item =>
                    item.Tenant.Slug.ToLower() == tenantSlug.ToLower() &&
                    item.Category.Slug.ToLower() == categorySlug.ToLower())
                .ToListAsync();
        }

        public async Task<List<ProductCategory>> GetCategoryListAsync(string tenantSlug)
        {
            return await _dbContext.Categories.OfType<ProductCategory>()
                .Include(item => item.Tenant)
                .Include(item => item.Image)
                .Where(item => item.Tenant.Slug.ToLower() == tenantSlug.ToLower())
                .ToListAsync();
        }
    }
}
