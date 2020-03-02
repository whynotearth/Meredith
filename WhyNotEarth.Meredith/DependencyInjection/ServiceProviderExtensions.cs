using Microsoft.Extensions.DependencyInjection;
using WhyNotEarth.Meredith.Hotel;
using WhyNotEarth.Meredith.Pages;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Stripe;

namespace WhyNotEarth.Meredith.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddMeredith(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IStripeService, StripeService>()
                .AddScoped<StripeOAuthService>()
                .AddScoped<ReservationService>()
                .AddScoped<CompanyService>()
                .AddScoped<PriceService>()
                .AddScoped<StoryService>()
                .AddScoped<IEmailService, EmailService>()
                .AddScoped<SendGridService>();
        }
    }
}