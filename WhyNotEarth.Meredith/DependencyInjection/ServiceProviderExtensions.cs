namespace WhyNotEarth.Meredith.DependencyInjection
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.Data.Entity.Models;
    using WhyNotEarth.Meredith.Hotel;
    using WhyNotEarth.Meredith.Identity;
    using WhyNotEarth.Meredith.Stripe;

    public static class ServiceProviderExtensions
    {

        public static IServiceCollection AddMeredith(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            return serviceCollection
                .AddIdentity<User, Role>()
                    .AddUserManager<UserManager>()
                    .AddEntityFrameworkStores<MeredithDbContext>()
                    .AddDefaultTokenProviders()
                .Services
                .AddScoped<StripeService>()
                .AddScoped<StripeOAuthService>()
                .AddScoped<ReservationService>();
        }
    }
}