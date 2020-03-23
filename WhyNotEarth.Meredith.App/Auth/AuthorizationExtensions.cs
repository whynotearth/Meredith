using Microsoft.Extensions.DependencyInjection;

namespace WhyNotEarth.Meredith.App.Auth
{
    public static class AuthorizationExtensions
    {
        public static void AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
                options.AddPolicy(Policies.Developer, p => p.RequireRole(Roles.Developer)));

            // Volkswagen
            services.AddPolicy(Policies.ManageJumpStart, Roles.VolkswagenAdmin);
        }

        private static void AddPolicy(this IServiceCollection services, string policy, string role)
        {
            services.AddAuthorization(options =>
                options.AddPolicy(policy, p => p.RequireRole(role, Roles.Developer)));
        }
    }
}