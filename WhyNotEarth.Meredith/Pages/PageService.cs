using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Pages
{
    public class PageService
    {
        private readonly MeredithDbContext _dbContext;

        public PageService(MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Page> GetPageAsync(string companySlug, string pageSlug)
        {
            return await Include().FirstOrDefaultAsync(p =>
                p.Company.Slug.ToLower() == companySlug.ToLower()
                && p.Slug!.ToLower() == pageSlug.ToLower());
        }

        public async Task<Page> GetPageAsync(string companySlug, string tenantSlug, string pageSlug)
        {
            return await Include().FirstOrDefaultAsync(p =>
                p.Company.Slug.ToLower() == companySlug.ToLower()
                && p.Tenant!.Slug.ToLower() == tenantSlug.ToLower()
                && p.Slug!.ToLower() == pageSlug.ToLower());
        }

        public async Task<List<Page>> GetPagesAsync(string companySlug)
        {
            return await Include().Where(p => p.Company.Slug.ToLower() == companySlug.ToLower())
                .ToListAsync();
        }

        public async Task<List<Page>> GetPagesAsync(string companySlug, string categoryName)
        {
            return await Include().Where(p => p.Company.Slug == companySlug && p.Category!.Name == categoryName)
                .ToListAsync();
        }

        private IQueryable<Page> Include()
        {
            return _dbContext.Pages
                .Include(p => p.Translations)
                .ThenInclude(p => p.Language)
                .Include(p => p.Company)
                .Include(p => p.Tenant)
                .Include(p => p.Cards)
                .Include(p => p.Category)
                .Include(p => p.Hotel)
                .ThenInclude(p => p!.Translations)
                .ThenInclude(p => p.Language)
                .Include(p => p.Hotel)
                .ThenInclude(p => p!.Amenities)
                .ThenInclude(p => p.Translations)
                .ThenInclude(p => p.Language)
                .Include(p => p.Hotel)
                .ThenInclude(p => p!.RoomTypes)
                .Include(p => p.Hotel)
                .ThenInclude(p => p!.RoomTypes)
                .ThenInclude(p => p.Beds)
                .Include(p => p.Hotel)
                .ThenInclude(p => p!.Rules)
                .ThenInclude(p => p.Translations)
                .ThenInclude(p => p.Language)
                .Include(p => p.Hotel)
                .ThenInclude(p => p!.Spaces)
                .ThenInclude(p => p.Translations)
                .ThenInclude(p => p.Language)
                .Include(p => p.Images);
        }

        public async Task<Page> GetLandingPageAsync(string companySlug, string pageSlug)
        {
            var page = await _dbContext.Pages.FirstOrDefaultAsync(p =>
                p.Company.Slug.ToLower() == companySlug.ToLower() && p.Slug!.ToLower() == pageSlug.ToLower());

            return page;
        }
    }
}