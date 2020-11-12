using Microsoft.Extensions.DependencyInjection;
using RoushTech.Xunit.EntityFrameworkCore;
using WhyNotEarth.Meredith.Persistence;

namespace WhyNotEarth.Meredith.Tests.Data
{
    public class DatabaseContextTest : DbContextTest<MeredithDbContext>
    {
        public DatabaseConfig DatabaseConfig { get; }

        protected DatabaseContextTest()
        {
            DatabaseConfig = DatabaseConfig.Instance;
            ServiceScope = DatabaseConfiguration.Instance.Services.CreateScope();
            DbContext = ServiceProvider.GetRequiredService<MeredithDbContext>();
            Setup();
        }
    }
}