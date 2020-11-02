using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WhyNotEarth.Meredith.App.Swagger
{
    internal class AcceptLanguageOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Accept-Language",
                In = ParameterLocation.Header,
                Description = "Supported languages",
                Schema = new OpenApiSchema
                {
                    Default = new OpenApiString("en"),
                    Type = "string",
                    Enum = Localization.Localization.SupportedCultures
                        .Select(c => OpenApiAnyFactory.CreateFor(new OpenApiSchema { Type = "string" }, c)).ToList()
                },
                Required = false
            });
        }
    }
}