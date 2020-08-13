using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Models;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.Shop
{
    public class ProductCategoryService
    {
        private readonly IDbContext _dbContext;
        private readonly TenantService _tenantService;

        public ProductCategoryService(IDbContext IDbContext, TenantService tenantService)
        {
            _dbContext = IDbContext;
            _tenantService = tenantService;
        }

        public async Task<List<ProductCategory>> ListAsync(string tenantSlug)
        {
            return await _dbContext.ProductCategories
                .Include(item => item.Tenant)
                .Include(item => item.Image)
                .Where(item => item.Tenant.Slug == tenantSlug)
                .ToListAsync();
        }

        public async Task<ProductCategory> GetAsync(int id)
        {
            var category = await _dbContext.ProductCategories
                .Include(item => item.Tenant)
                .Include(item => item.Image)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (category == null)
            {
                throw new RecordNotFoundException($"Category {id} not found");
            }

            return category;
        }

        public async Task DeleteAsync(int id, User user)
        {
            var category = await GetAsync(id);

            if (category is null)
            {
                throw new RecordNotFoundException($"Category {id} not found");
            }

            await _tenantService.CheckPermissionAsync(user, category.Tenant.Slug);

            _dbContext.ProductCategories.Remove(category);

            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateAsync(string tenantSlug, ProductCategoryModel model, User user)
        {
            var tenant = await _tenantService.CheckPermissionAsync(user, tenantSlug);

            var category = Map(new ProductCategory(), model, tenant);

            _dbContext.ProductCategories.Add(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ProductCategory> EditAsync(string tenantSlug, int categoryId, ProductCategoryModel model, User user)
        {
            var tenant = await _tenantService.CheckPermissionAsync(user, tenantSlug);

            var category = await _dbContext.ProductCategories
                .FirstOrDefaultAsync(item => item.Id == categoryId);

            if (category is null)
            {
                throw new RecordNotFoundException($"Category {categoryId} not found");
            }

            category = Map(category, model, tenant);

            _dbContext.ProductCategories.Update(category);
            await _dbContext.SaveChangesAsync();

            return category;
        }

        private ProductCategory Map(ProductCategory category, ProductCategoryModel model, Public.Tenant tenant)
        {
            category.Description = model.Description;
            category.Name = model.Name;
            category.TenantId = tenant.Id;

            if (model.ImageUrl != null)
            {
                category.Image = new CategoryImage
                {
                    Url = model.ImageUrl
                };
            }

            return category;
        }
    }
}