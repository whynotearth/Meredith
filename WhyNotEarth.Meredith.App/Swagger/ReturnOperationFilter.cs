using System.Net.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WhyNotEarth.Meredith.App.Swagger
{
    internal class ReturnOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.HttpMethod == HttpMethod.Get.ToString())
            {
                operation.Responses.TryAdd("401", new OpenApiResponse {Description = "Unauthorized"});
            }
        }
    }
}