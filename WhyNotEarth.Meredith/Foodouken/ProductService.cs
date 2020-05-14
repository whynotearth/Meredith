using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.Foodouken
{
    public class ProductService
    {
        private readonly MeredithDbContext _dbContext;

        public ProductService(MeredithDbContext meredithDbContext)
        {
            _dbContext = meredithDbContext;
        }
        public async Task<Product> GetProductAsync(int productId)
        {
            var product = await _dbContext.Products
                .Include(item => item.Images)
                .SingleOrDefaultAsync(item => item.Id == productId);

            if (product == null)
            {
                throw new RecordNotFoundException("Product not found.");
            }
            return product;
        }

        public async Task EditAsync(int productId, int tenantId, int categoryId, string name, string description, decimal price,
            string currency, ICollection<ProductImage> images)
        {
            if (tenantId <= 0)
            {
                throw new InvalidActionException($"Invalid TenantId.");
            }

            if (categoryId <= 0)
            {
                throw new InvalidActionException($"Invalid CategoryId.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidActionException($"Name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new InvalidActionException($"Currency cannot be empty.");
            }

            // TODO: validate the price.

            var product = await _dbContext.Products
                .Include(item => item.Images)
                .SingleOrDefaultAsync(item => item.Id == productId);

            if (product == null)
            {
                throw new RecordNotFoundException("Product does not exist.");
            }

            product.CategoryId = categoryId;
            product.TenantId = tenantId;
            product.Description = description;
            product.Price = price;
            product.Currency = currency;

            var updatedImages = product.Images.Where(item => images.Any(i => i.Id == item.Id)).ToList();
            var deletedImages = product.Images.Where(item => !updatedImages.Any(x => x.Id == item.Id)).ToList();
            var addedImages = images.Where(item => !updatedImages.Any(x => x.Id == item.Id)).ToList();

            addedImages.ForEach(item => product.Images.Add(item));
            deletedImages.ForEach(item => _dbContext.Images.Remove(item));
            updatedImages.ForEach(item =>
            {
                var updatedImage = images.SingleOrDefault(x => x.Id == item.Id);
                if (updatedImage != null)
                {
                    item.Order = updatedImage.Order;
                    item.Title = updatedImage.Title;
                    item.Url = updatedImage.Url;
                    item.AltText = updatedImage.AltText;
                }
            });

            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
        }
    }
}
