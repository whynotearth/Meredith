using Microsoft.Extensions.DependencyInjection;

namespace WhyNotEarth.Meredith.Persistence
{
    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IDbContext, MeredithDbContext>();

            return serviceCollection;
        }
    }
}