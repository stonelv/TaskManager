using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using TaskManager.API.Models;

namespace TaskManager.API.Exceptions
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred");

            var response = context.Response;
            response.ContentType = "application/json";

            ApiResponse apiResponse;

            switch (exception)
            {
                case UnauthorizedAccessException _:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    apiResponse = ApiResponse.ErrorResult(exception.Message, 401);
                    break;
                case KeyNotFoundException _:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    apiResponse = ApiResponse.ErrorResult(exception.Message, 404);
                    break;
                case InvalidOperationException _:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse = ApiResponse.ErrorResult(exception.Message, 400);
                    break;
                case ArgumentException _:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse = ApiResponse.ErrorResult(exception.Message, 400);
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    apiResponse = ApiResponse.ErrorResult("服务器内部错误", 500);
                    break;
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var result = JsonSerializer.Serialize(apiResponse, options);

            return response.WriteAsync(result);
        }
    }
}
