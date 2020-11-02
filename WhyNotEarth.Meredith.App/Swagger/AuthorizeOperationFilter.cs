using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WhyNotEarth.Meredith.App.Swagger
{
    internal class AuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var parentHasAuthorizeAttribute = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>().Any() ?? false;

            var methodHasAuthorizeAttribute = context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

            var hasAuthorizeAttribute = parentHasAuthorizeAttribute || methodHasAuthorizeAttribute;

            if (!hasAuthorizeAttribute)
            {
                return;
            }

            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
        }
    }
}