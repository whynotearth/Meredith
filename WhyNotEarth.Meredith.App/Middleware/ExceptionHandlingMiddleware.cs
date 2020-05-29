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
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment env)
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
                var response = GetResponse(exception);

                context.Response.Clear();
                context.Response.ContentType = "application/json";

                context.Response.StatusCode = response.StatusCode;
                var json = JsonConvert.SerializeObject(response.Result, new JsonSerializerSettings
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

        private Response GetResponse(Exception exception)
        {
            var statusCode = exception switch
            {
                InvalidActionException _ => StatusCodes.Status400BadRequest,
                DuplicateRecordException _ => StatusCodes.Status400BadRequest,
                ForbiddenException _ => StatusCodes.Status403Forbidden,
                RecordNotFoundException _ => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Unhandled exception");

                if (!_env.IsDevelopment())
                {
                    return new Response(statusCode, "Unexpected error :(");
                }

                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
            }

            return new Response(statusCode, exception);
        }

        private class Response
        {
            public int StatusCode { get; }

            public object Result { get; }

            public Response(int statusCode, string message)
            {
                StatusCode = statusCode;
                Result = GetResult(message);
            }

            public Response(int statusCode, Exception exception)
            {
                StatusCode = statusCode;

                if (exception is InvalidActionException invalidActionException && invalidActionException.Error != null)
                {
                    Result = invalidActionException.Error;
                }
                else
                {
                    Result = GetResult(exception.Message);
                }
            }

            private object GetResult(string message)
            {
                return new
                {
                    Message = message
                };
            }
        }
    }
}