using Microsoft.Extensions.DependencyInjection;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.GoogleCloud;
using WhyNotEarth.Meredith.Hotel;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Pages;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Services;
using WhyNotEarth.Meredith.Stripe;
using WhyNotEarth.Meredith.Volkswagen;

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
                .AddScoped<GoogleStorageService>();
            
            // Hotel
            serviceCollection
                .AddScoped<ReservationService>()
                .AddScoped<PriceService>()
                .AddScoped<IEmailService, EmailService>();

            // Volkswagen
            serviceCollection
                .AddScoped<ArticleService>()
                .AddScoped<JumpStartService>()
                .AddScoped<JumpStartEmailService>()
                .AddScoped<JumpStartPdfService>()
                .AddScoped<MemoService>()
                .AddScoped<JumpStartEmailTemplateService>()
                .AddScoped<RecipientService>()
                .AddScoped<EmailRecipientService>()
                .AddScoped<JumpStartPreviewService>()
                .AddScoped<PuppeteerService>()
                .AddScoped<JumpStartSendPlanService>();

            // Tenant
            serviceCollection
                .AddScoped<Tenant.ReservationService>();

            return serviceCollection;
        }
    }
}