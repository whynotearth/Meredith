using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoushTech.Xunit.EntityFrameworkCore;
using WhyNotEarth.Meredith.DependencyInjection;
using WhyNotEarth.Meredith.Persistence;
using WhyNotEarth.Meredith.Persistence.Models.Modules.Platform;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Services;
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
            var standardStripePlanId = await stripeSubscriptionService.GetPlanByName("Browtricks");
            var enterpriseStripePlanId = await stripeSubscriptionService.GetPlanByName("Browtricks 2: Electric Boogaloo");
            var standardPlan = await dbContext.PlatformPlans.FirstOrDefaultAsync(p => p.StripeId == standardStripePlanId);
            if (standardPlan == null)
            {
                dbContext.PlatformPlans.Add(new Plan
                {
                    Name = "Browtricks",
                    StripeId = standardStripePlanId!
                });
                await dbContext.SaveChangesAsync();
            }

            var enterprisePlan = await dbContext.PlatformPlans.FirstOrDefaultAsync(p => p.StripeId == enterpriseStripePlanId);
            if (enterprisePlan == null)
            {
                dbContext.PlatformPlans.Add(new Plan
                {
                    Name = "Browtricks 2: Electric Boogaloo",
                    StripeId = enterpriseStripePlanId!
                });
                await dbContext.SaveChangesAsync();
            }
        }
    }
}