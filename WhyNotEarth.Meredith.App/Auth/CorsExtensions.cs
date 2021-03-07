using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WhyNotEarth.Meredith.App.Auth
{
    public static class CorsExtensions
    {
        private static readonly List<(string? Production, string? Staging, string[] Netlify)> _projetcs = new List<(string?, string?, string[])>
        {
            (
                "https://foodouken.com",
                "https://staging.foodouken.com",
                new[] {"foodouken.netlify.app"}
            ),
            (
                "https://browtricksbeauty.com",
                "https://staging.browtricksbeauty.com",
                new[] {"browtricks.netlify.app"}
            ),
            (
                "https://vkirirom.com",
                "https://staging.vkirirom.com",
                new[] {"vk-mort.netlify.app", "cranky-nightingale-3731fc.netlify.app"}
            ),
            (
                "https://thebluedelta.com",
                "https://staging.thebluedelta.com",
                new[] {"eloquent-thompson-534232.netlify.app"}
            ),
            (
                null,
                "https://whynot.earth",
                Array.Empty<string>()
            ),
            (
                "https://sayabooking.com",
                null,
                new[] {"elegant-pare-29cad9.netlify.app"}
            ),
            (
                null,
                null,
                new[] {"romantic-swanson-ead31b.netlify.app"} // blobby
            ),
            // For Cordova
            (
                "capacitor://localhost",
                "capacitor://localhost",
                Array.Empty<string>()
            ),
            (
                "http://localhost",
                "http://localhost",
                Array.Empty<string>()
            )
        };

        public static void AddCustomCorsPolicy(this IServiceCollection services, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                services.AddCors(o => o
                    .AddDefaultPolicy(builder => builder
                        .SetIsOriginAllowed(origin => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()));

                return;
            }

            if (environment.IsStaging())
            {
                services.AddCors(o => o.AddDefaultPolicy(builder => builder
                    .SetIsOriginAllowed(IsOriginAllowed)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                ));

                return;
            }

            if (environment.IsProduction())
            {
                services.AddCors(o => o.AddDefaultPolicy(builder => builder
                    .WithOrigins(GetProductionDomains())
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                ));

                return;
            }

            throw new ArgumentException($"Invalid environment: {environment.EnvironmentName}", nameof(environment));
        }

        private static bool IsOriginAllowed(string origin)
        {
            if (_projetcs.Any(item => item.Staging == origin))
            {
                return true;
            }

            var uri = new Uri(origin);
            if (uri.Host == "localhost" || uri.Host == "bs-local.com")
            {
                return true;
            }

            return IsNetlify(origin);
        }

        private static bool IsNetlify(string origin)
        {
            const string patternPrefix = "https:\\/\\/(deploy-preview-\\d+--|[a-z0-9]+--)?";

            foreach (var project in _projetcs)
            {
                foreach (var domain in project.Netlify)
                {
                    var pattern = patternPrefix + Regex.Escape(domain);

                    if (Regex.IsMatch(origin, pattern))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static string[] GetProductionDomains()
        {
            return _projetcs.Select(item => item.Production).Where(item => item != null).ToArray()!;
        }
    }
}