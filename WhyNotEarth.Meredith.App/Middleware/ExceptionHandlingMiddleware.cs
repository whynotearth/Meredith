namespace WhyNotEarth.Meredith.App.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using WhyNotEarth.Meredith.Exceptions;

    public class ExceptionHandlingMiddleware
    {
        private RequestDelegate Next { get; }

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await Next(context);
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
            switch (exception)
            {
                case InvalidActionException invalidActionException:
                    httpResponse.StatusCode = 400;
                    return new
                    {
                        invalidActionException.Message
                    };
                case RecordNotFoundException recordNotFoundException:
                    httpResponse.StatusCode = 404;
                    return new
                    {
                        recordNotFoundException.Message
                    };
                default:
                    httpResponse.StatusCode = 500;
                    return new
                    {
                        exception.Message
                    };
            }
        }
    }
}