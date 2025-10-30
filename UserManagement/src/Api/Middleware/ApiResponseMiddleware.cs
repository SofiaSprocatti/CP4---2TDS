using System.Diagnostics;
using System.Net;
using System.Text.Json;
using UserManagement.Api.Models;

namespace UserManagement.Api.Middleware;

public class ApiResponseMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiResponseMiddleware> _logger;

    public ApiResponseMiddleware(RequestDelegate next, ILogger<ApiResponseMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await _next(context);
            stopwatch.Stop();
            
            if (!context.Response.HasStarted)
            {
                context.Response.Headers["X-Time-Elapsed"] = stopwatch.ElapsedMilliseconds.ToString();
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            await HandleExceptionAsync(context, ex, stopwatch.ElapsedMilliseconds);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, long timeElapsed)
    {
        _logger.LogError(exception, "Unhandled exception occurred");

        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Cannot handle exception, response has already started");
            return;
        }

        var response = ApiResponse.Failure(
            exception.Message,
            exception.StackTrace,
            timeElapsed
        );

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        await context.Response.WriteAsync(jsonResponse);
    }
}