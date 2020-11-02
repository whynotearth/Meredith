using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WhyNotEarth.Meredith.App.Swagger
{
    internal class ReturnCodeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true).ToList() ?? new List<object>();
            attributes.AddRange(context.MethodInfo.GetCustomAttributes(true));

            if (attributes.OfType<AuthorizeAttribute>().Any())
            {
                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
            }

            if (attributes.OfType<Returns404Attribute>().Any())
            {
                operation.Responses.TryAdd("404", new OpenApiResponse { Description = "Not Found" });
            }
        }
    }
}