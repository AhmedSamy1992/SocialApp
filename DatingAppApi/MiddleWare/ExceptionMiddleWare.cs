using DatingAppApi.Errors;
using System.Net;
using System.Text.Json;

namespace DatingAppApi.MiddleWare
{
    public class ExceptionMiddleWare(RequestDelegate next, ILogger<ExceptionMiddleWare> logger, IHostEnvironment env)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; //StatusCodes.Status500InternalServerError;

                var response = env.IsDevelopment()
                                ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace)
                                : new ApiException(context.Response.StatusCode, ex.Message, "Internal server Error");

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }

    }
}
