using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RollbarDotNet.Configuration;
using WhyNotEarth.Meredith.Cloudinary;
using WhyNotEarth.Meredith.GoogleCloud;
using WhyNotEarth.Meredith.Stripe.Data;

namespace WhyNotEarth.Meredith.App.Configuration
{
    public static class Options
    {
        public static void AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions()
                .Configure<CloudinaryOptions>(o => configuration.GetSection("Cloudinary").Bind(o))
                .Configure<RollbarOptions>(o => configuration.GetSection("Rollbar").Bind(o))
                .Configure<StripeOptions>(o => configuration.GetSection("Stripe").Bind(o))
                .Configure<JwtOptions>(o => configuration.GetSection("Jwt").Bind(o))
                .Configure<GoogleCloudOptions>(o => configuration.GetSection("GoogleCloud").Bind(o));
        }
    }
}