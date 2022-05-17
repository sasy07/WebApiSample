using Common;
using Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebApiSample.WebFramework.Api;

namespace WebApiSample.WebFramework.Middlewares;

public static class CustomExceptionHandlerMiddlewareExtensions
{
    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
}

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<CustomExceptionHandlerMiddleware> logger;

    public CustomExceptionHandlerMiddleware
        (RequestDelegate next, ILogger<CustomExceptionHandlerMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (AppException ex)
        {
            logger.LogError(ex, ex.Message);
            var apiResult = new ApiResult(false, ex.StatusCode);
            var json = JsonConvert.SerializeObject(apiResult);
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطایی رخ داده است");
            var apiResult = new ApiResult(false, ApiResultStatusCode.ServerError);
            var json = JsonConvert.SerializeObject(apiResult);
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(json);
        }
    }
}