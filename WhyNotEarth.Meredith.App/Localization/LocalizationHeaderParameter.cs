using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WhyNotEarth.Meredith.App.Localization
{
    internal class LocalizationHeaderParameter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            var localization = new Localization();

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "Accept-Language",
                In = "header",
                Enum = new List<object>(localization.SupportedCultures),
                Required = false
            });
        }
    }
}
