using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task CreateAsync(int pageId, int priceId, List<Variation> variations,
            List<ProductLocationInventory> productLocationInventories)
        {
            await ValidatePageAndPriceInputs(pageId, priceId);

            var product = new Product
            {
                PageId = pageId,
                PriceId = priceId,
                ProductLocationInventories = new List<ProductLocationInventory>(),
                Variations = new List<Variation>()
            };

            UpdateProductLocationInventories(default,
                product.ProductLocationInventories,
                !(productLocationInventories is null) ? productLocationInventories
                        : new List<ProductLocationInventory>());
            UpdateVariations(default,
                product.Variations,
                !(variations is null) ? variations
                        : new List<Variation>());

            await _dbContext.ShoppingProducts.AddAsync(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Product> EditAsync(int productId, int pageId, int priceId, ICollection<Variation> variations,
            ICollection<ProductLocationInventory> productLocationInventories)
        {
            Func<RecordNotFoundException> productNotFound = () => new RecordNotFoundException("Product does not exist.");

            if (productId <= 0)
            {
                throw productNotFound();
            }

            await ValidatePageAndPriceInputs(pageId, priceId);

            var product = await _dbContext.ShoppingProducts
                .Include(item => item.Variations)
                .Include(item => item.ProductLocationInventories)
                .FirstOrDefaultAsync(item => item.Id == productId);

            if (product is null)
            {
                throw productNotFound();
            }

            product.PageId = pageId;
            product.PriceId = priceId;


            UpdateVariations(productId, product.Variations, variations);
            UpdateProductLocationInventories(productId, product.ProductLocationInventories, productLocationInventories);

            _dbContext.ShoppingProducts.Update(product);
            await _dbContext.SaveChangesAsync();

            return product;
        }

        private async Task ValidatePageAndPriceInputs(int pageId, int priceId)
        {
            if (pageId <= 0 || !(await _dbContext.Pages.AnyAsync(item => item.Id == pageId)))
            {
                throw new InvalidActionException($"Invalid PageId.");
            }

            if (priceId <= 0 || !(await _dbContext.Prices.AnyAsync(item => item.Id == priceId)))
            {
                throw new InvalidActionException($"Invalid PriceId.");
            }
        }

        private void UpdateVariations(int productId, ICollection<Variation> savedCollection, ICollection<Variation> newCollection)
        {
            var updatedRecords = savedCollection.Where(item => newCollection.Any(i => i.Id == item.Id)).ToList();
            var deletedRecords = savedCollection.Where(item => !updatedRecords.Any(x => x.Id == item.Id)).ToList();
            var addedRecords = newCollection.Where(item => !updatedRecords.Any(x => x.Id == item.Id)).ToList();

            Action<Variation> validate = item =>
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    throw new InvalidActionException("The name of variation should not be empty.");
                }
            };

            addedRecords.ForEach(item =>
            {
                validate(item);
                savedCollection.Add(item);
            });
            deletedRecords.ForEach(item => _dbContext.Variations.Remove(item));
            updatedRecords.ForEach(item =>
            {
                var changedRecord = newCollection.FirstOrDefault(x => x.Id == item.Id);
                if (!(changedRecord is null))
                {
                    validate(item);
                    item.Name = changedRecord.Name;
                    item.ProductId = productId;
                }
            });
        }

        private void UpdateProductLocationInventories(int productId, ICollection<ProductLocationInventory> savedCollection,
            ICollection<ProductLocationInventory> newCollection)
        {
            var updatedRecords = savedCollection.Where(item => newCollection.Any(i => i.Id == item.Id)).ToList();
            var deletedRecords = savedCollection.Where(item => !updatedRecords.Any(x => x.Id == item.Id)).ToList();
            var addedRecords = newCollection.Where(item => !updatedRecords.Any(x => x.Id == item.Id)).ToList();

            Action<ProductLocationInventory> validate = item =>
            {
                // TODO: Validate changedRecord.Count
                if (item.LocationId <= 0 ||
                    ! _dbContext.Locations.Any(location => location.Id == item.LocationId))
                {
                    throw new InvalidActionException($"Invalid LocationId.");
                }
            };

            addedRecords.ForEach(item =>
            {
                validate(item);
                savedCollection.Add(item);
            });
            deletedRecords.ForEach(item => _dbContext.ProductLocationInventories.Remove(item));
            updatedRecords.ForEach(async item =>
            {
                var changedRecord = newCollection.FirstOrDefault(x => x.Id == item.Id);
                if (!(changedRecord is null))
                {
                    validate(item);
                    item.LocationId = changedRecord.LocationId;
                    item.ProductId = productId;
                    item.Count = changedRecord.Count;
                }
            });
        }
    }
}
