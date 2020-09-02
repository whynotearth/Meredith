using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.BrowTricks
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

        public async Task<List<ClientImage>> GetAllImages(User user)
        {
            return await _dbContext.Images
                .OfType<ClientImage>()
                .Include(item => item.Client)
                .ThenInclude(item => item!.Tenant)
                .Where(item => item.Client!.Tenant.OwnerId == user.Id)
                .ToListAsync();
        }

        public async Task<List<ClientVideo>> GetAllVideos(User user)
        {
            return await _dbContext.Videos
                .OfType<ClientVideo>()
                .Include(item => item.Client)
                .ThenInclude(item => item!.Tenant)
                .Where(item => item.Client!.Tenant.OwnerId == user.Id)
                .ToListAsync();
        }

        public async Task<List<ClientImage>> GetAllImages(string tenantSlug, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            return await _dbContext.Images
                .OfType<ClientImage>()
                .Include(item => item.Client)
                .Where(item => item.Client!.TenantId == tenant.Id)
                .ToListAsync();
        }

        public async Task<List<ClientVideo>> GetAllVideos(string tenantSlug, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            return await _dbContext.Videos
                .OfType<ClientVideo>()
                .Include(item => item.Client)
                .Where(item => item.Client!.TenantId == tenant.Id)
                .ToListAsync();
        }
    }
}