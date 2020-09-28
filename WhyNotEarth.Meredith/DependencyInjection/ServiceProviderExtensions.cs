using Microsoft.Extensions.DependencyInjection;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.BrowTricks.Jobs;
using WhyNotEarth.Meredith.BrowTricks.Services;
using WhyNotEarth.Meredith.Cloudinary;
using WhyNotEarth.Meredith.Emails;
using WhyNotEarth.Meredith.Emails.Jobs;
using WhyNotEarth.Meredith.GoogleCloud;
using WhyNotEarth.Meredith.Hotel;
using WhyNotEarth.Meredith.Identity;
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
                .AddScoped<IFormSignatureFileService, FormSignatureFileService>()
                .AddScoped<ITwilioService, TwilioService>()
                .AddScoped<IMarkdownService, MarkdownService>()
                .AddScoped<IHtmlService, HtmlService>()
                .AddScoped<ILoginTokenService, LoginTokenService>();

            // Hotel
            serviceCollection
                .AddScoped<ReservationService>()
                .AddScoped<PriceService>()
                .AddScoped<IEmailService, EmailService>();

            // Shop
            serviceCollection
                .AddScoped<TenantService>()
                .AddScoped<ProductService>()
                .AddScoped<ProductCategoryService>()
                .AddScoped<SeoSchemaService>();

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
                .AddScoped<IFormAnswerService, FormAnswerService>()
                .AddScoped<IClientSaveSignatureJob, ClientSaveSignatureJob>()
                .AddScoped<ClientNoteService>()
                .AddScoped<FormNotifications>()
                .AddScoped<IBrowTricksService, BrowTricksService>()
                .AddScoped<IFormTemplateService, FormTemplateService>()
                .AddScoped<IFormSignatureService, FormSignatureService>();

            return serviceCollection;
        }
    }
}