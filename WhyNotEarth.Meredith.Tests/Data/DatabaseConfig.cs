namespace WhyNotEarth.Meredith.Tests.Data
{
    using System.Threading.Tasks;
    using RoushTech.Xunit.EntityFrameworkCore;

    public class DatabaseConfig
    {
        public static DatabaseConfig Instance { get; } = new DatabaseConfig();

        private DatabaseConfig()
        {
            DatabaseConfiguration.Instance.CreateServiceProvider();
            DatabaseConfiguration.Instance.CreateDatabases();
            Task.Run(Seed).Wait();
        }

        private async Task Seed()
        {
        }
    }
}