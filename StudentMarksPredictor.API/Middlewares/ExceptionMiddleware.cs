using System.Text.Json;
using StudentMarksPredictor.API.DTOs;
using StudentMarksPredictor.Shared.Exceptions;

namespace StudentMarksPredictor.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (BaseException ex)
        {
            if (ex is LoggableException)
                _logger.LogError(ex, ex.Message);

            await WriteErrorResponse(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Beklenmeyen bir hata olustu");
            await WriteErrorResponse(context, 500, "Sunucu hatasi");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = ApiResponse<object>.Fail(message);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
