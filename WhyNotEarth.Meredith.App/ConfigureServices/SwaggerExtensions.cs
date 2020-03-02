using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using WhyNotEarth.Meredith.App.Localization;

namespace WhyNotEarth.Meredith.App.ConfigureServices
{
    public static class SwaggerExtensions
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v0", new Info
                {
                    Title = "Interface API",
                    Version = "v0",
                    Description =
                        "API designed for internal use only, will change and WILL break backwards compability as needed for our GUI"
                });
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    apiDesc.TryGetMethodInfo(out var methodInfo);
                    var versions = methodInfo.DeclaringType.GetCustomAttributes(true)
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);
                    return versions.Any(v => $"v{v.ToString()}" == docName);
                });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }}
                };
                c.AddSecurityRequirement(security);
                c.OperationFilter<LocalizationHeaderParameter>();
            });
        }
    }
}
