using Microsoft.AspNetCore.Authorization;
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
            services.AddPolicy(Policies.ManageVolkswagen, Roles.VolkswagenManager);

            // Shop
            services.AddPolicy<ManageTenantHandler, ManageTenantRequirement>(Policies.ManageTenant,
                new ManageTenantRequirement());
        }

        private static void AddPolicy(this IServiceCollection services, string policy, string role)
        {
            services.AddAuthorization(options =>
                options.AddPolicy(policy, p => p.RequireRole(role, Roles.Developer)));
        }

        private static void AddPolicy<THandler, TRequirement>(this IServiceCollection services, string policy, TRequirement requirement)
            where THandler : AuthorizationHandler<TRequirement>
            where TRequirement : IAuthorizationRequirement
        {
            services.AddAuthorization(options => { options.AddPolicy(policy, p => p.Requirements.Add(requirement)); });

            services.AddScoped<IAuthorizationHandler, THandler>();
        }
    }
}