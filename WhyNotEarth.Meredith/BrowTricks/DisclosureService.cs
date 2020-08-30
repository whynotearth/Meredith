using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.BrowTricks
{
    internal class DisclosureService : IDisclosureService
    {
        private readonly IDbContext _dbContext;
        private readonly TenantService _tenantService;

        public DisclosureService(IDbContext dbContext, TenantService tenantService)
        {
            _dbContext = dbContext;
            _tenantService = tenantService;
        }

        public async Task CreateAsync(string tenantSlug, DisclosureModel model, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            var disclosures = await _dbContext.Disclosures.Where(item => item.TenantId == tenant.Id).ToListAsync();

            // Remove the deleted items
            var ids = model.Disclosures.Where(item => item.Id.HasValue).Select(item => item.Id).ToList();
            var removedOnes = disclosures.Where(item => !ids.Contains(item.Id)).ToList();
            _dbContext.Disclosures.RemoveRange(removedOnes);

            foreach (var disclosureItemModel in model.Disclosures)
            {
                if (disclosureItemModel.Id.HasValue)
                {
                    var disclosure = disclosures.FirstOrDefault(item => item.Id == disclosureItemModel.Id);

                    if (disclosure is null)
                    {
                        throw new InvalidActionException();
                    }

                    disclosure.Value = disclosureItemModel.Value;
                    _dbContext.Disclosures.Update(disclosure);
                }
                else
                {
                    _dbContext.Disclosures.Add(new Disclosure
                    {
                        TenantId = tenant.Id,
                        Value = disclosureItemModel.Value
                    });
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Disclosure>> ListAsync(string tenantSlug, User user)
        {
            await ValidateOwnerOrClient(tenantSlug, user);

            return await _dbContext.Disclosures
                .Include(item => item.Tenant)
                .Where(item => item.Tenant.Slug == tenantSlug)
                .ToListAsync();
        }

        private async Task ValidateOwnerOrClient(string tenantSlug, User user)
        {
            var tenant = await _dbContext.Tenants
                .FirstOrDefaultAsync(item => item.Slug == tenantSlug && item.OwnerId == user.Id);

            // It's the owner
            if (tenant != null)
            {
                return;
            }

            var client = await _dbContext.Clients
                .Include(item => item.Tenant)
                .FirstOrDefaultAsync(item => item.UserId == user.Id && item.Tenant.Slug == tenantSlug);

            if (client != null)
            {
                return;
            }

            throw new ForbiddenException();
        }
    }
}