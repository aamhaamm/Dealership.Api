using System.Net;
using System.Text.Json;

namespace Dealership.Api.Middleware;

/// <summary>
/// Global error handling middleware that ensures consistent JSON responses.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                code = "INTERNAL_ERROR",
                message = "An unexpected error occurred. Please try again later."
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
