using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace WhyNotEarth.Meredith.App.Auth
{
    public static class CorsExtensions
    {
        public static void AddCustomCorsPolicy(this IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddCors(o => o
                .AddDefaultPolicy(builder => builder
                    .SetIsOriginAllowed(origin => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()));
        }
    }
}