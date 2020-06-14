using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Models;

namespace WhyNotEarth.Meredith.Shop
{
    public class ProductCategoryService
    {
        private readonly MeredithDbContext _dbContext;

        public ProductCategoryService(MeredithDbContext meredithDbContext)
        {
            _dbContext = meredithDbContext;
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
            await CheckPermissionAsync(user, id);

            var category = await GetAsync(id);

            _dbContext.ProductCategories.Remove(category);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<ProductCategory> CreateAsync(ProductCategoryModel model, User user)
        {
            var tenant = await GetTenant(model.TenantSlug!);
            if(tenant is null)
            {
                throw new InvalidActionException($"Tenant {model.TenantSlug} not found");
            }

            await CheckPermissionAsync(user, tenant.Id);

            var category = new ProductCategory
            {
                Description = model.Description,
                Name = model.Name,
                TenantId = tenant.Id,
                ImageURL = model.ImageURL
            };

            _dbContext.ProductCategories.Add(category);
            await _dbContext.SaveChangesAsync();

            return category;
        }

        public async Task<ProductCategory> EditAsync(int categoryId, ProductCategoryModel model, User user)
        {
            var tenant = await GetTenant(model.TenantSlug!);
            if (tenant is null)
            {
                throw new InvalidActionException($"Tenant {model.TenantSlug} not found");
            }

            await CheckPermissionAsync(user, tenant.Id);

            var category = await _dbContext.ProductCategories
                .FirstOrDefaultAsync(item => item.Id == categoryId);

            if(category is null)
            {
                throw new RecordNotFoundException($"Category {categoryId} not found");
            }

            category.Name = model.Name;
            category.Description = model.Description;
            category.ImageURL = model.ImageURL;

            _dbContext.ProductCategories.Update(category);
            await _dbContext.SaveChangesAsync();

            return category;
        }

        private async Task<Data.Entity.Models.Tenant> GetTenant(string tenantSlug)
        {
            return await _dbContext.Tenants.FirstOrDefaultAsync(item =>
                item.Slug == tenantSlug);
        }

        private async Task CheckPermissionAsync(User user, int tenantId)
        {
            var ownsTenant = await _dbContext.Tenants.AnyAsync(item => item.Id == tenantId && item.OwnerId == user.Id);

            if (!ownsTenant)
            {
                throw new ForbiddenException("You don't own this tenant");
            }
        }
    }
}