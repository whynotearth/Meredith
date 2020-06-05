using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.Shop
{
    public class ProductService
    {
        private readonly MeredithDbContext _dbContext;

        public ProductService(MeredithDbContext meredithDbContext)
        {
            _dbContext = meredithDbContext;
        }

        public Task<List<Product>> ListAsync()
        {
            return _dbContext.ShoppingProducts
                .Include(item => item.Variations)
                .Include(item => item.ProductLocationInventories)
                .ToListAsync();
        }

        public async Task<Product> GetAsync(int productId)
        {
            var product = await _dbContext.ShoppingProducts
                .Include(item => item.ProductLocationInventories)
                .Include(item => item.Variations)
                .FirstOrDefaultAsync(item => item.Id == productId);

            if (product == null)
            {
                throw new RecordNotFoundException($"Product {productId} not found");
            }

            return product;
        }

        public async Task DeleteAsync(int productId)
        {
            var product = await GetAsync(productId);

            _dbContext.ShoppingProducts.Remove(product);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Product> CreateAsync(int pageId, int priceId, int categoryId, List<Variation> variations,
            List<ProductLocationInventory> productLocationInventories)
        {
            await Validate(pageId, priceId, categoryId, variations, productLocationInventories);

            var product = new Product
            {
                CategoryId = categoryId,
                PageId = pageId,
                PriceId = priceId,
                ProductLocationInventories = new List<ProductLocationInventory>(),
                Variations = new List<Variation>()
            };

            product.Variations = variations;
            product.ProductLocationInventories = productLocationInventories;

            _dbContext.ShoppingProducts.Add(product);
            await _dbContext.SaveChangesAsync();

            return product;
        }

        public async Task<Product> EditAsync(int productId, int pageId, int priceId, int categoryId, List<Variation> variations,
            List<ProductLocationInventory> productLocationInventories)
        {
            await Validate(pageId, priceId, categoryId, variations, productLocationInventories);

            var product = await _dbContext.ShoppingProducts
                .Include(item => item.Variations)
                .Include(item => item.ProductLocationInventories)
                .FirstOrDefaultAsync(item => item.Id == productId);

            product.CategoryId = categoryId;
            product.PageId = pageId;
            product.PriceId = priceId;
            product.Variations = variations;
            product.ProductLocationInventories = productLocationInventories;

            _dbContext.ShoppingProducts.Update(product);
            await _dbContext.SaveChangesAsync();

            return product;
        }

        private async Task Validate(int pageId, int priceId, int categoryId, List<Variation> variations,
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

            if (!await _dbContext.Prices.AnyAsync(item => item.Id == priceId))
            {
                throw new RecordNotFoundException($"Price {priceId} not found");
            }

            variations.ForEach(item =>
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    throw new InvalidActionException("Variation name cannot be empty");
                }
            });

            foreach (var productLocationInventory in productLocationInventories)
            {
                if (!await _dbContext.Locations.AnyAsync(location => location.Id == productLocationInventory.LocationId))
                {
                    throw new RecordNotFoundException($"Company {productLocationInventory.LocationId} not found");
                }
            }
        }
    }
}