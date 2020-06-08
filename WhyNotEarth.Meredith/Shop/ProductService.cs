using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Models;
using Product = WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop.Product;

namespace WhyNotEarth.Meredith.Shop
{
    public class ProductService
    {
        private readonly MeredithDbContext _dbContext;

        public ProductService(MeredithDbContext meredithDbContext)
        {
            _dbContext = meredithDbContext;
        }

        public Task<List<Product>> ListAsync(int categoryId)
        {
            return _dbContext.ShoppingProducts
                .Include(item => item.Price)
                .Include(item => item.Variations)
                    .ThenInclude(item => item.Price)
                .Include(item => item.ProductLocationInventories)
                .Include(item => item.ProductAttributes)
                    .ThenInclude(item => item.Price)
                .Where(item => item.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<Product> GetAsync(int productId)
        {
            var product = await _dbContext.ShoppingProducts
                .Include(item => item.Price)
                .Include(item => item.Variations)
                    .ThenInclude(item => item.Price)
                .Include(item => item.ProductLocationInventories)
                .Include(item => item.ProductAttributes)
                    .ThenInclude(item => item.Price)
                .FirstOrDefaultAsync(item => item.Id == productId);

            if (product == null)
            {
                throw new RecordNotFoundException($"Product {productId} not found");
            }

            return product;
        }

        public async Task DeleteAsync(int productId, User user)
        {
            var product = await _dbContext.ShoppingProducts
                .Include(item => item.Category)
                .ThenInclude(item => item.Tenant)
                .FirstOrDefaultAsync(item => item.Id == productId);

            await CheckPermissionAsync(user, product.Category.TenantId);

            _dbContext.ShoppingProducts.Remove(product);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Product> CreateAsync(int categoryId, ProductModel model, User user)
        {
            var category = await _dbContext.ProductCategories.FirstOrDefaultAsync(item => item.Id == categoryId);
            await CheckPermissionAsync(user, category.TenantId);

            var variations = model.Variations.Select(item =>
                new Variation
                {
                    Price = new Price
                    {
                        Amount = item.Price!.Value
                    },
                    Name = item.Name
                }).ToList();

            var productLocationInventories = model.LocationInventories.Select(item =>
                new ProductLocationInventory
                {
                    Count = item.Count!.Value,
                    Location = new Location { Name = model.Name } // TODO: What is the desired value for the field?
                }).ToList();

            var productAttributes = model.Attributes.Select(item =>
                new ProductAttribute
                {
                    Name = item.Name,
                    Price = new Price
                    {
                        Amount = item.Price!.Value
                    }
                }).ToList();

            await ValidateAsync(model.PageId!.Value, categoryId, variations);

            var product = new Product
            {
                Name = model.Name,
                CategoryId = categoryId,
                PageId = model.PageId.Value,
                Price = new Price { Amount = model.Price!.Value },
                ProductLocationInventories = productLocationInventories,
                Variations = variations,
                ProductAttributes = productAttributes
            };

            _dbContext.ShoppingProducts.Add(product);
            await _dbContext.SaveChangesAsync();

            return product;
        }

        public async Task<Product> EditAsync(int productId, int categoryId, ProductModel model, User user)
        {
            var category = await _dbContext.ProductCategories.FirstOrDefaultAsync(item => item.Id == categoryId);
            await CheckPermissionAsync(user, category.TenantId);

            var product = await _dbContext.ShoppingProducts
                .Include(item => item.Price)
                .Include(item => item.Variations)
                .Include(item => item.ProductLocationInventories)
                .Include(item => item.ProductAttributes)
                .FirstOrDefaultAsync(item => item.Id == productId);

            var variations = model.Variations.Select(item =>
                new Variation
                {
                    Name = item.Name,
                    Price = new Price
                    {
                        Amount = item.Price!.Value
                    },
                }).ToList();

            var productLocationInventories = model.LocationInventories.Select(item =>
                new ProductLocationInventory
                {
                    Count = item.Count!.Value,
                    LocationId = item.LocationId!.Value
                }).ToList();

            var productAttributes = model.Attributes.Select(item =>
                new ProductAttribute
                {
                    Name = item.Name,
                    Price = new Price
                    { 
                        Amount = item.Price!.Value
                    }
                }).ToList();

            await ValidateAsync(model.PageId!.Value, categoryId, variations);

            product.Price.Amount = model.Price!.Value;
            product.Name = model.Name;
            product.PageId = model.PageId.Value;
            product.Variations = variations;
            product.ProductLocationInventories = productLocationInventories;
            product.ProductAttributes = productAttributes;

            _dbContext.ShoppingProducts.Update(product);
            await _dbContext.SaveChangesAsync();

            return product;
        }

        private async Task ValidateAsync(int pageId, int categoryId, List<Variation> variations)
        {
            if (!await _dbContext.Pages.AnyAsync(item => item.Id == pageId))
            {
                throw new RecordNotFoundException($"Page {pageId} not found");
            }

            if (!await _dbContext.ProductCategories.AnyAsync(item => item.Id == categoryId))
            {
                throw new RecordNotFoundException($"Category {pageId} not found");
            }

            variations.ForEach(item =>
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    throw new InvalidActionException("Variation name cannot be empty");
                }
            });
        }

        private async Task CheckPermissionAsync(User user, int tenantId)
        {
            var ownsTenant = await _dbContext.Tenants.AnyAsync(item => item.Id == tenantId && item.UserId == user.Id);

            if (!ownsTenant)
            {
                throw new ForbiddenException("You don't own this tenant");
            }
        }
    }
}