namespace WhyNotEarth.Meredith.Tests.Platform
{
    using System.Linq;
    using System.Threading.Tasks;
    using Faker;
    using Microsoft.EntityFrameworkCore;
    using WhyNotEarth.Meredith.Public;
    using WhyNotEarth.Meredith.Tests.Data;

    public class BasePlatformTests : DatabaseContextTest
    {
        public BasePlatformTests()
        {
        }

        protected async Task<Meredith.Public.Tenant> CreateTenant()
        {
            var tenant = new Meredith.Public.Tenant
            {
                Owner = new User
                {
                    FirstName = Name.First(),
                    LastName = Name.Last(),
                    Email = Internet.Email(),
                }
            };
            DbContext.Add(tenant);
            await DbContext.SaveChangesAsync();
            return tenant;
        }

        protected async Task<Plan> GetPlan(string planName)
        {
            return await DbContext.PlatformPlans
                .Include(p => p.Platform)
                .ThenInclude(p => p.Company)
                .ThenInclude(c => c!.StripeAccount)
                .FirstOrDefaultAsync(p => p.Name == planName);
        }
    }
}