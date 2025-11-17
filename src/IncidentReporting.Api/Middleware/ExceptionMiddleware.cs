using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace IncidentReporting.Api.Middleware
{
    /// <summary>
    /// Global error handler for API.
    /// Converts exceptions into consistent JSON responses.
    /// Handles validation errors, concurrency issues, and generic failures.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch (ValidationException vex)
            {
                _logger.LogWarning("Validation error: {Errors}", vex.Errors);

                var errors = vex.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                });

                await WriteJsonResponse(
                    context,
                    HttpStatusCode.BadRequest,
                    new { Message = "Validation Failed", Errors = errors });

            }
            catch (DbUpdateConcurrencyException cex)
            {
                _logger.LogWarning("Concurrency conflict: {Message}", cex.Message);

                await WriteJsonResponse(
                    context,
                    HttpStatusCode.Conflict,
                    new { Message = "Concurrency Conflict. The record was modified by another user." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                await WriteJsonResponse(
                    context,
                    HttpStatusCode.InternalServerError,
                    new { Message = "An unexpected error occurred." });
            }
        }

        private static async Task WriteJsonResponse(
            HttpContext context,
            HttpStatusCode statusCode,
            object responseObj)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(responseObj,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await context.Response.WriteAsync(json);
        }
    }
}
