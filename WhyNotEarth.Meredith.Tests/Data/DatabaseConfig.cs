using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using RoushTech.Xunit.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.DependencyInjection;
using WhyNotEarth.Meredith.Persistence;

namespace WhyNotEarth.Meredith.Tests.Data
{
    public class DatabaseConfig
    {
        public static DatabaseConfig Instance { get; } = new DatabaseConfig();

        private DatabaseConfig()
        {
            DatabaseConfiguration.Instance.ServiceCollection
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

        private Task Seed()
        {
            return Task.CompletedTask;
        }
    }
}