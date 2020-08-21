using Microsoft.Extensions.DependencyInjection;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.BrowTricks.Jobs;
using WhyNotEarth.Meredith.Cloudinary;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.GoogleCloud;
using WhyNotEarth.Meredith.HelloSign;
using WhyNotEarth.Meredith.Hotel;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Jobs.Public;
using WhyNotEarth.Meredith.Jobs.Volkswagen;
using WhyNotEarth.Meredith.Makrdown;
using WhyNotEarth.Meredith.Pages;
using WhyNotEarth.Meredith.Pdf;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Services;
using WhyNotEarth.Meredith.Shop;
using WhyNotEarth.Meredith.Stripe;
using WhyNotEarth.Meredith.Tenant;
using WhyNotEarth.Meredith.Twilio;
using WhyNotEarth.Meredith.Volkswagen;
using WhyNotEarth.Meredith.Volkswagen.Jobs;
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
                .AddScoped<UserManager>()
                .AddScoped<GoogleStorageService>()
                .AddScoped<SettingsService>()
                .AddScoped<EmailRecipientService>()
                .AddScoped<EmailRecipientJob>()
                .AddScoped<SlugService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IFileService, FileService>()
                .AddScoped<ICloudinaryService, CloudinaryService>()
                .AddScoped<IHelloSignService, HelloSignService>()
                .AddScoped<ITwilioService, TwilioService>()
                .AddScoped<IMarkdownService, MarkdownService>()
                .AddScoped<IHtmlService, HtmlService>();

            // Hotel
            serviceCollection
                .AddScoped<ReservationService>()
                .AddScoped<PriceService>()
                .AddScoped<IEmailService, EmailService>();

            // Shop
            serviceCollection
                .AddScoped<TenantService>()
                .AddScoped<ProductService>()
                .AddScoped<ProductCategoryService>();

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
                .AddScoped<JumpStartPlanService>()
                .AddScoped<JumpStartMarkdownService>()
                .AddScoped<NewJumpStartService>()
                .AddScoped<NewJumpStartJob>()
                .AddScoped<NewJumpStartEmailJob>();

            // Tenant
            serviceCollection
                .AddScoped<Tenant.ReservationService>()
                .AddScoped<TenantReservationNotification>();

            // BrowTricks
            serviceCollection
                .AddScoped<IClientService, ClientService>()
                .AddScoped<IPmuService, PmuService>()
                .AddScoped<IClientSaveSignatureJob, ClientSaveSignatureJob>()
                .AddScoped<ClientNoteService>()
                .AddScoped<PmuNotifications>();

            return serviceCollection;
        }
    }
}