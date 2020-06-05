using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Models.Page;

namespace WhyNotEarth.Meredith.Public
{
    public class PageCategoryService
    {
        private readonly MeredithDbContext _dbContext;
        public PageCategoryService(MeredithDbContext meredithDbContext)
        {
            _dbContext = meredithDbContext;
        }

        public async Task<List<PageCategory>> ListAsync(string tenantSlug)
        {
            return await _dbContext.Tenants.Include(x => x.Pages)
                .Where(c => c.Slug == tenantSlug)
                .SelectMany(x => x.Pages)
                .Include(x => x.Images)
                .Select(x => x.Category)
                .ToListAsync();
        }

        public async Task<PageCategory> CreateAsync(PageCategoryCreateModel model)
        {
            await ValidateAsync(model.Name!);

            var category = new PageCategory
            {
                Description = model.Description,
                Name = model.Name,
                Slug = model.Slug,
            };

            _dbContext.PageCategories.Add(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task DeleteAsync(int id)
        {
            var category = await GetAsync(id);

            _dbContext.PageCategories.Remove(category);

            await _dbContext.SaveChangesAsync();
        }

        public async Task EditAsync(PageCategoryEditModel model)
        {
            var category = await GetAsync(model.Id);

            await ValidateAsync(model.Name!);

            category.Name = model.Name;
            category.Slug = model.Slug;
            category.Description = model.Description;

            _dbContext.PageCategories.Update(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PageCategory> GetAsync(int id)
        {
            var category = await _dbContext.PageCategories
                .Include(x => x.Image)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category is null)
            {
                throw new RecordNotFoundException($"Category {id} not found");
            }

            return category;
        }

        private async Task ValidateAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidActionException($"Empty name is not allowed.");
            }
            var duplicatedName = await _dbContext.PageCategories.AnyAsync(x => x.Name == name);

            if (duplicatedName)
            {
                throw new DuplicateRecordException($"Name '{name}' is already exist.");
            }
        }
    }
}
