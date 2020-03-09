using Microsoft.Extensions.DependencyInjection;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Hotel;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Pages;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Services;
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
                .AddScoped<SendGridService>()
                .AddScoped<IUserManager, UserManager>();
        }
    }
}