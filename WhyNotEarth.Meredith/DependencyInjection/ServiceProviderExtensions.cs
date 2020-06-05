using Microsoft.Extensions.DependencyInjection;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.GoogleCloud;
using WhyNotEarth.Meredith.Hotel;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Jobs.Public;
using WhyNotEarth.Meredith.Jobs.Volkswagen;
using WhyNotEarth.Meredith.Pages;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Services;
using WhyNotEarth.Meredith.Shop;
using WhyNotEarth.Meredith.Sms;
using WhyNotEarth.Meredith.Stripe;
using WhyNotEarth.Meredith.Tenant;
using WhyNotEarth.Meredith.Volkswagen;
using ReservationService = WhyNotEarth.Meredith.Hotel.ReservationService;

namespace WhyNotEarth.Meredith.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddMeredith(this IServiceCollection serviceCollection)
        {
            // Public
            serviceCollection
                .AddScoped<IStripeService, StripeService>()
                .AddScoped<StripeOAuthService>()
                .AddScoped<CompanyService>()
                .AddScoped<PageService>()
                .AddScoped<SendGridService>()
                .AddScoped<IUserManager, UserManager>()
                .AddScoped<GoogleStorageService>()
                .AddScoped<SettingsService>()
                .AddScoped<EmailRecipientService>()
                .AddScoped<EmailRecipientJob>()
                .AddScoped<SlugService>()
                .AddScoped<UserService>()
                .AddScoped<ProductCategoryService>();

            // Hotel
            serviceCollection
                .AddScoped<ReservationService>()
                .AddScoped<PriceService>()
                .AddScoped<IEmailService, EmailService>();

            // Shop
            serviceCollection
                .AddScoped<TenantService>()
                .AddScoped<ProductService>()
                .AddScoped<ClientService>();

            // Volkswagen
            serviceCollection
                .AddScoped<ArticleService>()
                .AddScoped<JumpStartService>()
                .AddScoped<JumpStartEmailJob>()
                .AddScoped<JumpStartPdfJob>()
                .AddScoped<MemoService>()
                .AddScoped<JumpStartEmailTemplateService>()
                .AddScoped<RecipientService>()
                .AddScoped<JumpStartPreviewService>()
                .AddScoped<PuppeteerService>()
                .AddScoped<JumpStartPlanService>()
                .AddScoped<JumpStartMarkdownService>();

            // Tenant
            serviceCollection
                .AddScoped<Tenant.ReservationService>()
                .AddScoped<TwilioService>();

            return serviceCollection;
        }
    }
}