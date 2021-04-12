using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RollbarDotNet.Configuration;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Cloudinary;
using WhyNotEarth.Meredith.GoogleCloud;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Stripe.Data;
using WhyNotEarth.Meredith.Twilio;

namespace WhyNotEarth.Meredith.App.Configuration
{
    public static class OptionsExtensions
    {
        public static void AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions()
                .Configure<CloudinaryOptions>(o => configuration.GetSection("Cloudinary").Bind(o))
                .Configure<RollbarOptions>(o => configuration.GetSection("Rollbar").Bind(o))
                .Configure<StripeOptions>(o => configuration.GetSection("Stripe").Bind(o))
                .Configure<JwtOptions>(o => configuration.GetSection("Jwt").Bind(o))
                .Configure<GoogleCloudOptions>(o => configuration.GetSection("GoogleCloud").Bind(o))
                .Configure<TwilioOptions>(o => configuration.GetSection("Twilio").Bind(o))
                .Configure<BrowTricksPlatformConfiguration>(o => configuration.GetSection("Platfomrms:BrowTricks").Bind(o));
        }
    }
}