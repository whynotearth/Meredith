using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Identity;

namespace WhyNotEarth.Meredith.App.Controllers
{
    public abstract class TenantControllerBase : ControllerBase
    {
        public TenantControllerBase(
            IDbContext dbContext,
            UserManager userManager
        )
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        protected readonly IDbContext _dbContext;

        protected readonly UserManager _userManager;

        protected async Task<WhyNotEarth.Meredith.Public.Tenant> GetUserOwnedBySlug(string slug)
        {
            var userId = _userManager.GetUserId(User);
            return await _dbContext.Tenants.FirstOrDefaultAsync(t => t.Slug == slug && t.OwnerId == userId);
        }
    }
}