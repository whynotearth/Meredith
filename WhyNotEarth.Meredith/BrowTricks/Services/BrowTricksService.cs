using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    internal class BrowTricksService : IBrowTricksService
    {
        private readonly IDbContext _dbContext;
        private readonly TenantService _tenantService;

        public BrowTricksService(IDbContext dbContext, TenantService tenantService)
        {
            _dbContext = dbContext;
            _tenantService = tenantService;
        }

        public async Task<List<BrowTricksImage>> GetAllImages(User user)
        {
            return await _dbContext.Images
                .OfType<BrowTricksImage>()
                .Include(item => item.Tenant)
                .Where(item => item.Tenant!.OwnerId == user.Id)
                .ToListAsync();
        }

        public async Task<List<BrowTricksVideo>> GetAllVideos(User user)
        {
            return await _dbContext.Videos
                .OfType<BrowTricksVideo>()
                .Include(item => item.Tenant)
                .Where(item => item.Tenant!.OwnerId == user.Id)
                .ToListAsync();
        }

        public async Task<List<BrowTricksImage>> GetAllImages(string tenantSlug, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            return await _dbContext.Images
                .OfType<BrowTricksImage>()
                .Include(item => item.Tenant)
                .Where(item => item.TenantId == tenant.Id)
                .ToListAsync();
        }

        public async Task<List<BrowTricksVideo>> GetAllVideos(string tenantSlug, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            return await _dbContext.Videos
                .OfType<BrowTricksVideo>()
                .Include(item => item.Tenant)
                .Where(item => item.TenantId == tenant.Id)
                .ToListAsync();
        }
    }
}