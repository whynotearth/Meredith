using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WhyNotEarth.Meredith.Cloudinary;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Models;
using WhyNotEarth.Meredith.Public;
using Page = WhyNotEarth.Meredith.Data.Entity.Models.Page;
using Product = WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop.Product;

namespace WhyNotEarth.Meredith.Shop
{
    public class ProductService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly SlugService _slugService;
        private readonly CloudinaryOptions _cloudinaryOptions;

        public ProductService(MeredithDbContext meredithDbContext, SlugService slugService, IOptions<CloudinaryOptions> cloudinaryOptions)
        {
            _dbContext = meredithDbContext;
            _slugService = slugService;
            _cloudinaryOptions = cloudinaryOptions.Value;
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

            await CheckPermissionAsync(product.Category, user);

            _dbContext.ShoppingProducts.Remove(product);

            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateAsync(int categoryId, ProductModel model, User user)
        {
            var (productCategory, tenant) = await ValidateAsync(categoryId, user, model.Variations);

            var product = Map(new Product(), productCategory, tenant, model);

            _dbContext.ShoppingProducts.Add(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Product> EditAsync(int productId, int categoryId, ProductModel model, User user)
        {
            bool deleteCloudinaryImage;
            var (productCategory, tenant) = await ValidateAsync(categoryId, user, model.Variations);

            var product = await _dbContext.ShoppingProducts
                    .Include(item => item.Page)
                    .Include(item => item.Image)
                    .Include(item => item.Price)
                    .Include(item => item.Variations)
                        .ThenInclude(item => item.Price)
                    .Include(item => item.ProductLocationInventories)
                        .ThenInclude(item => item.Location)
                    .Include(item => item.ProductAttributes)
                        .ThenInclude(item => item.Price)
                    .FirstOrDefaultAsync(item => item.Id == productId);

            var productImageUrl = product.Image is null ? null : product.Image!.Url;
            product = MapEdit(product, productCategory, tenant, model, out deleteCloudinaryImage);

            if (deleteCloudinaryImage)
            {
                DeleteCloudinaryImage(productImageUrl);
            }

            _dbContext.ShoppingProducts.Update(product);
            await _dbContext.SaveChangesAsync();            

            return product;
        }

        private async Task<(ProductCategory, Data.Entity.Models.Tenant)> ValidateAsync(int categoryId, User user, List<VariationModel>? variationModels)
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

        private async Task<Data.Entity.Models.Tenant> CheckPermissionAsync(ProductCategory category, User user)
        {
            var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(item => item.Id == category.TenantId && item.OwnerId == user.Id);
            if (tenant is null)
            {
                throw new ForbiddenException("You don't own this tenant");
            }

            return tenant;
        }

        private Product Map(Product product, ProductCategory category, Data.Entity.Models.Tenant tenant, ProductModel model)
        {
            product.Name = model.Name;
            product.CategoryId = category.Id;
            product.Price = new Price {Amount = model.Price!.Value};
            product.Description = model.Description;
            product.IsAvailable = model.IsAvailable!.Value;

            if (model.ImageUrl != null)
            {
                product.Image = new ProductImage
                {
                    Url = model.ImageUrl
                };
            }

            if (product.Page is null)
            {
                product.Page = new Page
                {
                    CompanyId = tenant.CompanyId,
                    TenantId =  tenant.Id,
                    Slug =  _slugService.GetSlug(product.Name),
                    CreationDateTime = DateTime.UtcNow
                };
            }

            product.Variations = model.Variations?.Select(item =>
                new Variation
                {
                    Price = new Price
                    {
                        Amount = item.Price!.Value
                    },
                    Name = item.Name
                }).ToList();

            product.ProductLocationInventories = model.LocationInventories?.Select(item =>
                new ProductLocationInventory
                {
                    Count = item.Count!.Value,
                    Location = new Location { Name = model.Name } // TODO: What is the desired value for the field?
                }).ToList();

            product.ProductAttributes = model.Attributes?.Select(item =>
                new ProductAttribute
                {
                    Name = item.Name,
                    Price = new Price
                    {
                        Amount = item.Price!.Value
                    }
                }).ToList();

            return product;
        }

        private Product MapEdit(Product product, ProductCategory category, Data.Entity.Models.Tenant tenant, ProductModel model, out bool deleteCloudinaryImage)
        {
            deleteCloudinaryImage = false;
            product.Name = model.Name;
            product.CategoryId = category.Id;
            product.Price.Amount = model.Price!.Value;
            product.Description = model.Description;
            product.IsAvailable = model.IsAvailable!.Value;

            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                if (product.Image is null)
                {
                    product.Image = new ProductImage
                    {
                        Url = model.ImageUrl
                    };
                }
                else
                {
                    product.Image.Url = model.ImageUrl;
                }

            }
            else
            {
                deleteCloudinaryImage = true;
                _dbContext.Images.Remove(product.Image);
                product.Image = null;
            }

            if (product.Page is null)
            {
                product.Page = new Page
                {
                    CompanyId = tenant.CompanyId,
                    TenantId = tenant.Id,
                    Slug = _slugService.GetSlug(product.Name),
                    CreationDateTime = DateTime.UtcNow
                };
            }

            UpdateChild(product, model);

            return product;
        }

        private void UpdateChild(Product product, ProductModel model)
        {
            foreach (var child in product.Variations.ToList())
            {
                if (!model.Variations.Any(item => item.Id == child.Id))
                {
                    _dbContext.Prices.Remove(child.Price);
                    _dbContext.Variations.Remove(child);
                }
            }

            foreach (var item in model.Variations.ToList())
            {
                var child = product.Variations.SingleOrDefault(p => p.Id == item.Id && p.Id != 0);
                if (child != null)
                {
                    child.Price.Amount = item.Price!.Value;
                    child.Name = item.Name;

                    _dbContext.Variations.Update(child);
                }
                else
                {
                    var variation = new Variation
                    {
                        Price = new Price
                        {
                            Amount = item.Price!.Value
                        },
                        Name = item.Name
                    };
                    product.Variations.Add(variation);
                }
            }

            foreach (var child in product.ProductAttributes.ToList())
            {
                if (!model.Attributes.Any(item => item.Id == child.Id))
                {
                    _dbContext.Prices.Remove(child.Price);
                    _dbContext.ProductAttributes.Remove(child);
                }
            }

            foreach (var item in model.Attributes.ToList())
            {
                var child = product.ProductAttributes.SingleOrDefault(p => p.Id == item.Id && p.Id != 0);
                if (child != null)
                {
                    child.Price.Amount = item.Price!.Value;
                    child.Name = item.Name;

                    _dbContext.ProductAttributes.Update(child);
                }
                else
                {
                    var attributes = new ProductAttribute
                    {
                        Price = new Price
                        {
                            Amount = item.Price!.Value
                        },
                        Name = item.Name
                    };
                    product.ProductAttributes.Add(attributes);
                }
            }

            foreach (var child in product.ProductLocationInventories.ToList())
            {
                if (!model.LocationInventories.Any(item => item.Id == child.Id))
                {
                    _dbContext.Locations.Remove(child.Location);
                    _dbContext.ProductLocationInventories.Remove(child);
                }
            }

            foreach (var item in model.LocationInventories.ToList())
            {
                var child = product.ProductLocationInventories.SingleOrDefault(p => p.Id == item.Id && p.Id != 0);
                if (child != null)
                {
                    child.LocationId = item.LocationId!.Value;
                    child.Count = item.Count!.Value;

                    _dbContext.ProductLocationInventories.Update(child);
                }
                else
                {
                    var locationInventories = new ProductLocationInventory
                    {
                        Location = new Location
                        {
                            Name = product.Name
                        },
                        Count = item.Count!.Value
                    };
                    product.ProductLocationInventories.Add(locationInventories);
                }
            }
        }

        private string DeleteCloudinaryImage(string imageUrl)
        {
            var cloudinary = new CloudinaryDotNet.Cloudinary(new Account(_cloudinaryOptions.CloudName,
            _cloudinaryOptions.ApiKey, _cloudinaryOptions.ApiSecret));

            var publicId = Path.GetFileNameWithoutExtension(imageUrl);

            var deleteParams = new DeletionParams(publicId);
            var result = cloudinary.Destroy(deleteParams);

            return result.Result;
        }
    }
}