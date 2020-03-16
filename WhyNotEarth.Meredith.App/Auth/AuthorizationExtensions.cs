using Microsoft.Extensions.DependencyInjection;

namespace WhyNotEarth.Meredith.App.Auth
{
    public static class AuthorizationExtensions
    {
        public static void AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
                options.AddPolicy(Policies.CreatePage, policy => policy.RequireRole(Roles.Admin)));
        }
    }
}