using System.Net;
using System.Text.Json;
using My_Store.Domain.Exceptions;

namespace My_Store.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }



        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, title) = exception switch
            {
                InvalidProductException _ => (HttpStatusCode.BadRequest, "Invalid Product"),
                ProductNotFoundException _ => (HttpStatusCode.NotFound, "Product Not Found"),
                _ => (HttpStatusCode.InternalServerError, "Internal Server Error")
            };

            _logger.LogError(exception, "Unhandled Exception: {Title}, StatusCode: {StatusCode}", title, (int)statusCode);

            var response = new
            {
                title,
                status = (int)statusCode,
                detail = exception.Message
            };

            var payload = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsync(payload);
        }
    }
}
