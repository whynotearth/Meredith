using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoushTech.Xunit.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Platform;
using WhyNotEarth.Meredith.DependencyInjection;
using WhyNotEarth.Meredith.Persistence;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Services;
using WhyNotEarth.Meredith.Stripe;
using WhyNotEarth.Meredith.Stripe.Data;

namespace WhyNotEarth.Meredith.Tests.Data
{
    public class DatabaseConfig
    {
        public static DatabaseConfig Instance { get; } = new DatabaseConfig();

        private DatabaseConfig()
        {
            DatabaseConfiguration.Instance.ServiceCollection
                .Configure<StripeOptions>(o => DatabaseConfiguration.Instance.Configuration.GetSection("Stripe").Bind(o))
                .AddMeredith()
                .AddPersistence()
                .AddDbContext<MeredithDbContext>(options => options
                    .UseInMemoryDatabase("test")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)))
                .AddScoped(o => new ClaimsPrincipal());
            DatabaseConfiguration.Instance.CreateServiceProvider();
            DatabaseConfiguration.Instance.CreateDatabases();
            Task.Run(Seed).Wait();
        }

        private async Task Seed()
        {
            var stripeSubscriptionService = DatabaseConfiguration.Instance.Services.GetRequiredService<IStripeSubscriptionService>();
            var dbContext = DatabaseConfiguration.Instance.Services.GetRequiredService<MeredithDbContext>();

            var platform = new Meredith.Public.Platform
            {
                Name = "Browtricks",
                Domain = "browtricks.com",
                SalesCut = 50,
                Company = new Company
                {
                    Name = "RoushTech, LLC",
                    StripeAccount = new StripeAccount
                    {
                        StripeUserId = "acct_102q8y2cvONgMs2J"
                    }
                }
            };
            var standardStripePlanId = await stripeSubscriptionService.GetPriceByDescription("Browtricks", platform.Company.StripeAccount.StripeUserId);
            var enterpriseStripePlanId = await stripeSubscriptionService.GetPriceByDescription("Browtricks 2: Electric Boogaloo", platform.Company.StripeAccount.StripeUserId);
            var standardPlan = await dbContext.PlatformPlans.FirstOrDefaultAsync(p => p.StripeId == standardStripePlanId);
            if (standardPlan == null)
            {
                dbContext.PlatformPlans.Add(new Plan
                {
                    Name = "Browtricks",
                    StripeId = standardStripePlanId!,
                    Platform = platform
                });
                await dbContext.SaveChangesAsync();
            }

            var enterprisePlan = await dbContext.PlatformPlans.FirstOrDefaultAsync(p => p.StripeId == enterpriseStripePlanId);
            if (enterprisePlan == null)
            {
                dbContext.PlatformPlans.Add(new Plan
                {
                    Name = "Browtricks 2: Electric Boogaloo",
                    StripeId = enterpriseStripePlanId!,
                    Platform = platform
                });
                await dbContext.SaveChangesAsync();
            }
        }
    }
}