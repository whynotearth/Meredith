using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace WhyNotEarth.Meredith.App.Localization
{
    internal class LocalizationHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Accept-Language",
                In = ParameterLocation.Header,
                Description = "Supported languages",
                Schema = new OpenApiSchema
                {
                    Default = new OpenApiString("en"),
                    Type = "string",
                    Enum = Localization.SupportedCultures
                        .Select(c => OpenApiAnyFactory.CreateFor(new OpenApiSchema { Type = "string" }, c)).ToList()
                },
                Required = false
            });
        }
    }
}