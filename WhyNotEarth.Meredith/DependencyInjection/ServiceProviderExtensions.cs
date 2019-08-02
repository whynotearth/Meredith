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
                .AddScoped<StripeServices>()
                .AddScoped<StripeOAuthServices>();
        }
    }
}