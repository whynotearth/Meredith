using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Cloudinary;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Shop.Models;
using Product = WhyNotEarth.Meredith.Shop.Product;

namespace WhyNotEarth.Meredith.Shop
{
    public class ProductService
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IDbContext _dbContext;
        private readonly SlugService _slugService;

        public ProductService(IDbContext IDbContext, SlugService slugService,
            ICloudinaryService cloudinaryService)
        {
            _dbContext = IDbContext;
            _slugService = slugService;
            _cloudinaryService = cloudinaryService;
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
                .Include(item => item.Image)
                .Include(item => item.Category)
                    .ThenInclude(item => item.Tenant)
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
                .Include(item => item.Image)
                .Include(item => item.Category)
                    .ThenInclude(item => item.Tenant)
                .FirstOrDefaultAsync(item => item.Id == productId);

            if (product is null)
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

            await CheckPermissionAsync(product.Category, user);

            _dbContext.ShoppingProducts.Remove(product);

            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateAsync(int categoryId, ProductModel model, User user)
        {
            var (productCategory, tenant) = await ValidateAsync(categoryId, user, model.Variations);

            var product = await MapAsync(new Product(), productCategory, tenant, model);

            _dbContext.ShoppingProducts.Add(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EditAsync(int productId, int categoryId, ProductModel model, User user)
        {
            var (productCategory, tenant) = await ValidateAsync(categoryId, user, model.Variations);

            var product = await _dbContext.ShoppingProducts
                .Include(item => item.Image)
                .Include(item => item.Price)
                .Include(item => item.Variations)
                .ThenInclude(item => item.Price)
                .Include(item => item.ProductLocationInventories)
                .ThenInclude(item => item.Location)
                .Include(item => item.ProductAttributes)
                .ThenInclude(item => item.Price)
                .FirstOrDefaultAsync(item => item.Id == productId);

            product = await MapAsync(product, productCategory, tenant, model);

            _dbContext.ShoppingProducts.Update(product);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<(ProductCategory, Public.Tenant)> ValidateAsync(int categoryId, User user,
            List<VariationModel>? variationModels)
        {
            var category = await _dbContext.ProductCategories.FirstOrDefaultAsync(item => item.Id == categoryId);
            if (category is null)
            {
                throw new RecordNotFoundException($"Category {categoryId} not found");
            }

            var tenant = await CheckPermissionAsync(category, user);

            variationModels?.ForEach(item =>
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    throw new InvalidActionException("Variation name cannot be empty");
                }
            });

            return (category, tenant);
        }

        private async Task<Public.Tenant> CheckPermissionAsync(ProductCategory category, User user)
        {
            var tenant =
                await _dbContext.Tenants.FirstOrDefaultAsync(item =>
                    item.Id == category.TenantId && item.OwnerId == user.Id);

            if (tenant is null)
            {
                throw new ForbiddenException("You don't own this tenant");
            }

            return tenant;
        }

        private async Task<Product> MapAsync(Product product, ProductCategory category,
            Public.Tenant tenant,
            ProductModel model)
        {
            product.Name = model.Name;
            product.CategoryId = category.Id;
            product.Price = new Price { Amount = model.Price!.Value };
            product.Description = model.Description;
            product.IsAvailable = model.IsAvailable!.Value;

            await UpdateImageAsync(product, model);

            product.Page ??= new Page
            {
                CompanyId = tenant.CompanyId,
                TenantId = tenant.Id,
                Slug = _slugService.GetSlug(product.Name),
                CreatedAt = DateTime.UtcNow
            };

            product.Variations = model.Variations?.Select(item =>
                new Variation
                {
                    Id = item.Id ?? default,
                    Price = new Price
                    {
                        Amount = item.Price!.Value
                    },
                    Name = item.Name
                }).ToList();

            product.ProductLocationInventories = model.LocationInventories?.Select(item =>
                new ProductLocationInventory
                {
                    Id = item.Id ?? default,
                    Count = item.Count!.Value,
                    Location = new Location { Name = model.Name } // TODO: What is the desired value for the field?
                }).ToList();

            product.ProductAttributes = model.Attributes?.Select(item =>
                new ProductAttribute
                {
                    Id = item.Id ?? default,
                    Name = item.Name,
                    Price = new Price
                    {
                        Amount = item.Price!.Value
                    }
                }).ToList();

            return product;
        }

        private async Task UpdateImageAsync(Product product, ProductModel model)
        {
            if (product.Image is null)
            {
                if (model.ImageUrl != null)
                {
                    product.Image = new ProductImage
                    {
                        Url = model.ImageUrl,
                        CreatedAt = DateTime.UtcNow
                    };
                }
            }
            else
            {
                if (model.ImageUrl != null)
                {
                    // Delete old image
                    await _cloudinaryService.DeleteByUrlAsync(product.Image.Url);

                    product.Image.Url = model.ImageUrl;
                }
                else
                {
                    await _cloudinaryService.DeleteByUrlAsync(product.Image.Url);
                    product.Image = null;
                }
            }
        }
    }
}