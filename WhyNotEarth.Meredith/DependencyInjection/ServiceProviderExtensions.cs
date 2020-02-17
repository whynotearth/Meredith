using WhyNotEarth.Meredith.Services;

namespace WhyNotEarth.Meredith.DependencyInjection
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using WhyNotEarth.Meredith.Cloudinary;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.Data.Entity.Models;
    using WhyNotEarth.Meredith.Email;
    using WhyNotEarth.Meredith.Hotel;
    using WhyNotEarth.Meredith.Identity;
    using WhyNotEarth.Meredith.Pages;
    using WhyNotEarth.Meredith.Public;
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
                .Configure<CloudinaryOptions>(o => configuration.GetSection("Cloudinary").Bind(o))
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