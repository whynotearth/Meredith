namespace WhyNotEarth.Meredith.Tests.Data
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.Extensions.DependencyInjection;
    using RoushTech.Xunit.EntityFrameworkCore;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.DependencyInjection;

    public class DatabaseConfig
    {
        public static DatabaseConfig Instance { get; } = new DatabaseConfig();

        private DatabaseConfig()
        {
            DatabaseConfiguration.Instance.ServiceCollection
                .AddMeredith(DatabaseConfiguration.Instance.Configuration)
                .AddDbContext<MeredithDbContext>(options => options
                    .UseInMemoryDatabase("test")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)))
                .AddScoped<ClaimsPrincipal>(o => new ClaimsPrincipal());
            DatabaseConfiguration.Instance.CreateServiceProvider();
            DatabaseConfiguration.Instance.CreateDatabases();
            Task.Run(Seed).Wait();
        }

        private async Task Seed()
        {
        }
    }
}