using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.App.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                context.Response.Clear();
                context.Response.ContentType = "application/json";

                var response = PopulateResponse(exception, context.Response);
                
                var json = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    },
                    Formatting = Formatting.None
                });

                await context.Response.WriteAsync(json);
            }
        }

        private object PopulateResponse(Exception exception, HttpResponse httpResponse)
        {
            httpResponse.StatusCode = exception switch
            {
                InvalidActionException _ => StatusCodes.Status400BadRequest,
                DuplicateRecordException _ => StatusCodes.Status400BadRequest,
                RecordNotFoundException _ => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            if (httpResponse.StatusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Unhandled exception");

                if (!_env.IsDevelopment())
                {
                    return new
                    {
                        Message = "Unexpected error :("
                    };
                }

                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
            }

            return new
            {
                exception.Message
            };
        }
    }
}