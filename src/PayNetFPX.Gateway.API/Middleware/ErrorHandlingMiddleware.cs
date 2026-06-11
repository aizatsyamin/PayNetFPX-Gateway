namespace PayNetFPX.Gateway.API.Middleware;

using System.Text.Json;
using Serilog;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred");
            await HandleExceptionAsync(context, exception);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            error = new
            {
                code = "INTERNAL_ERROR",
                message = exception.Message,
                requestId = context.TraceIdentifier,
                timestamp = DateTime.UtcNow
            }
        };

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return context.Response.WriteAsJsonAsync(response);
    }
}
