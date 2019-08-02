namespace WhyNotEarth.Meredith.DependencyInjection
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
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