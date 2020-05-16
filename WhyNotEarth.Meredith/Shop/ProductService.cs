using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<List<Product>> ListAsync(int? priceId, int? pageId)
        {
            var products = _dbContext.ShoppingProducts
                .Include(item => item.Variations)
                .Include(item => item.ProductLocationInventories)
                .Where(item => true);

            if (!(priceId is null))
            {
                products = products.Where(item => item.PriceId == priceId);
            }

            if (!(pageId is null))
            {
                products = products.Where(item => item.PageId == pageId);
            }

            return await products.ToListAsync();
        }

        public async Task<Product> GetAsync(int productId)
        {
            var product = await _dbContext.ShoppingProducts
                .Include(item => item.ProductLocationInventories)
                .Include(item => item.Variations)
                .FirstOrDefaultAsync(item => item.Id == productId);

            if (product == null)
            {
                throw new RecordNotFoundException("Product not found.");
            }
            return product;
        }

        public async Task DeleteAsync(int productId)
        {
            var product = await GetAsync(productId);
            _dbContext.ShoppingProducts.Remove(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Product> CreateAsync(int pageId, int priceId, List<Variation> variations,
            List<ProductLocationInventory> productLocationInventories)
        {
            await Validate(pageId, priceId, variations, productLocationInventories);

            var product = new Product
            {
                PageId = pageId,
                PriceId = priceId,
                ProductLocationInventories = new List<ProductLocationInventory>(),
                Variations = new List<Variation>()
            };

            variations.ForEach(item =>
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    throw new InvalidActionException("Invalid Variation name.");
                }
            });

            productLocationInventories.ForEach(item =>
            {
                if (item.LocationId <= 0 ||
                    !_dbContext.Locations.Any(location => location.Id == item.LocationId))
                {
                    throw new InvalidActionException("Invalid LocationId.");
                }
            });

            product.Variations = variations;
            product.ProductLocationInventories = productLocationInventories;

            await _dbContext.ShoppingProducts.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            return product;
        }

        public async Task<Product> EditAsync(int productId, int pageId, int priceId, List<Variation> variations,
            List<ProductLocationInventory> productLocationInventories)
        {
            await Validate(pageId, priceId, variations, productLocationInventories);

            var product = await _dbContext.ShoppingProducts
                .Include(item => item.Variations)
                .Include(item => item.ProductLocationInventories)
                .FirstOrDefaultAsync(item => item.Id == productId);

            if (product is null)
            {
                throw new RecordNotFoundException("Product does not exist.");
            }

            product.PageId = pageId;
            product.PriceId = priceId;

            variations.ForEach(item =>
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    throw new InvalidActionException("Invalid Variation name.");
                }
            });

            productLocationInventories.ForEach(item =>
            {
                if (item.LocationId <= 0 ||
                    !_dbContext.Locations.Any(location => location.Id == item.LocationId))
                {
                    throw new InvalidActionException("Invalid LocationId.");
                }
            });

            product.Variations = variations;
            product.ProductLocationInventories = productLocationInventories;

            _dbContext.ShoppingProducts.Update(product);
            await _dbContext.SaveChangesAsync();

            return product;
        }

        private async Task Validate(int pageId, int priceId, List<Variation> variations,
            List<ProductLocationInventory> productLocationInventories)
        {
            if (pageId <= 0 || !(await _dbContext.Pages.AnyAsync(item => item.Id == pageId)))
            {
                throw new InvalidActionException($"Invalid PageId.");
            }

            if (priceId <= 0 || !(await _dbContext.Prices.AnyAsync(item => item.Id == priceId)))
            {
                throw new InvalidActionException($"Invalid PriceId.");
            }

            variations.ForEach(item =>
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    throw new InvalidActionException("Invalid Variation name.");
                }
            });

            productLocationInventories.ForEach(item =>
            {
                if (item.LocationId <= 0 ||
                    !_dbContext.Locations.Any(location => location.Id == item.LocationId))
                {
                    throw new InvalidActionException("Invalid LocationId.");
                }
            });
        }
    }
}