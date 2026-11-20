using Application.DTO.Response;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var statusCode = StatusCodes.Status500InternalServerError;
            var message = exception.Message;

            if (exception is ValidationException validationException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                message = "Validation failed: " + string.Join(", ", validationException.Errors.Select(e => e.ErrorMessage));
            }

            context.Response.StatusCode = statusCode;

            var response = new ServicesResponse(true, message);
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
