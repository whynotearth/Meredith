using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Models.Shop;

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
                .Where(item => item.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<Product> GetAsync(int categoryId, int productId)
        {
            var product = await _dbContext.ShoppingProducts
                .Include(item => item.Price)
                .Include(item => item.ProductLocationInventories)
                .Include(item => item.Variations)
                    .ThenInclude(item => item.Price)
                .FirstOrDefaultAsync(item => item.CategoryId == categoryId && item.Id == productId);

            if (product == null)
            {
                throw new RecordNotFoundException($"Product {productId} not found");
            }

            return product;
        }

        public async Task DeleteAsync(int categoryId, int productId)
        {
            var product = await GetAsync(categoryId, productId);

            _dbContext.ShoppingProducts.Remove(product);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Product> CreateAsync(ProductCreateModel model)
        {
            var variations = model.Variations.Select(item =>
                new Variation
                {
                    Price = new Price
                    {
                        Amount = item.Price
                    },
                    Name = item.Name
                }).ToList();

            var productLocationInventories = model.ProductLocationInventories.Select(item =>
                new ProductLocationInventory
                {
                    Count = item.Count,
                    Location = new Location { Name = model.Name }, // What is the desired value for the field?
                }).ToList();

            await Validate(model.PageId, model.CategoryId, variations, productLocationInventories);

            var product = new Product
            {
                Name = model.Name,
                CategoryId = model.CategoryId,
                PageId = model.PageId,
                Price = new Price { Amount = model.Price },
                ProductLocationInventories = productLocationInventories,
                Variations = variations
            };

            _dbContext.ShoppingProducts.Add(product);
            await _dbContext.SaveChangesAsync();

            return product;
        }

        public async Task<Product> EditAsync(ProductEditModel model)
        {
            var variations = model.Variations.Select(item =>
                new Variation
                {
                    Id = item.Id,
                    Name = item.Name,
                    PriceId = item.PriceId,
                    Price = new Price
                    {
                        Amount = item.Price,
                        Id = item.PriceId
                    },
                }).ToList();

            var productLocationInventories = model.ProductLocationInventories.Select(item =>
                new ProductLocationInventory
                {
                    Id = item.Id,
                    Count = item.Count,
                    LocationId = item.LocationId
                }).ToList();
            await Validate(model.PageId, model.CategoryId, variations, productLocationInventories);

            var product = await _dbContext.ShoppingProducts
                .Include(item => item.Price)
                .Include(item => item.Variations)
                .Include(item => item.ProductLocationInventories)
                .FirstOrDefaultAsync(item => item.Id == model.Id);

            product.Price.Amount = model.Price;
            product.Name = model.Name;
            product.CategoryId = model.CategoryId;
            product.PageId = model.PageId;
            product.Variations = variations;
            product.ProductLocationInventories = productLocationInventories;

            _dbContext.ShoppingProducts.Update(product);
            await _dbContext.SaveChangesAsync();

            return product;
        }

        private async Task Validate(int pageId, int categoryId, List<Variation> variations,
            List<ProductLocationInventory> productLocationInventories)
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

            foreach (var productLocationInventory in productLocationInventories.Where(item => item.Id > 0))
            {
                if (!await _dbContext.Locations.AnyAsync(location => location.Id == productLocationInventory.LocationId))
                {
                    throw new RecordNotFoundException($"Company {productLocationInventory.LocationId} not found");
                }
            }
        }
    }
}