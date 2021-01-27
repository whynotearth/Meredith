using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace WhyNotEarth.Meredith.App.Auth
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
                options.AddPolicy(Policies.Developer, p => p.RequireRole(Roles.Developer)));

            // Volkswagen
            services.AddPolicy(Policies.ManageVolkswagen, Roles.VolkswagenManager);

            // Shop
            return services.AddPolicy<ManageTenantHandler, ManageTenantRequirement>(Policies.ManageTenant,
                new ManageTenantRequirement());
        }

        private static IServiceCollection AddPolicy(this IServiceCollection services, string policy, string role)
        {
            return services.AddAuthorization(options =>
                options.AddPolicy(policy, p => p.RequireRole(role, Roles.Developer)));
        }

        private static IServiceCollection AddPolicy<THandler, TRequirement>(this IServiceCollection services, string policy, TRequirement requirement)
            where THandler : AuthorizationHandler<TRequirement>
            where TRequirement : IAuthorizationRequirement
        {
            services.AddAuthorization(options => { options.AddPolicy(policy, p => p.Requirements.Add(requirement)); });

            return services.AddScoped<IAuthorizationHandler, THandler>();
        }
    }
}