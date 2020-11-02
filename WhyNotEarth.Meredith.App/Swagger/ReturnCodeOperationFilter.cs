using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WhyNotEarth.Meredith.App.Mvc;

namespace WhyNotEarth.Meredith.App.Swagger
{
    internal class ReturnCodeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true).ToList() ?? new List<object>();
            attributes.AddRange(context.MethodInfo.GetCustomAttributes(true));

            if (context.MethodInfo.ReturnType == typeof(Task<CreateResult>))
            {
                operation.Responses.TryAdd("201", new OpenApiResponse { Description = "Success" });
                operation.Responses.Remove("200");
            }
            else if (context.MethodInfo.ReturnType == typeof(Task<CreateObjectResult>))
            {
                // TODO: Define the object schema
                operation.Responses.TryAdd("201", new OpenApiResponse { Description = "Success" });
                operation.Responses.Remove("200");
            }

            if (attributes.OfType<Returns400Attribute>().Any())
            {
                operation.Responses.TryAdd("400", new OpenApiResponse { Description = "Bad Request" });
            }

            if (attributes.OfType<AuthorizeAttribute>().Any())
            {
                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
            }
            else if (attributes.OfType<Returns401Attribute>().Any())
            {
                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            }

            if (attributes.OfType<Returns404Attribute>().Any())
            {
                operation.Responses.TryAdd("404", new OpenApiResponse { Description = "Not Found" });
            }
        }
    }
}