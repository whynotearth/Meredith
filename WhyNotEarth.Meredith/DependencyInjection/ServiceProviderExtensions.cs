namespace WhyNotEarth.Meredith.DependencyInjection
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.Stripe;

    public static class ServiceProviderExtensions
    {

        public static IServiceCollection AddMeredith(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            return serviceCollection
                .AddDbContext<MeredithDbContext>(o => o.UseNpgsql(configuration.GetConnectionString("Default"),
                    options => options.SetPostgresVersion(new Version(9, 6))))
                .AddScoped<StripeServices>()
                .AddScoped<StripeOAuthServices>();
        }
    }
}