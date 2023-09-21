using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using SocialMedia.Exceptions;

namespace SocialMedia.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ILogger<ExceptionHandlingMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (AggregateException ae)
        {
            foreach (var e in ae.InnerExceptions)
            {
                await HandleExceptionAsync(context, e, logger);
            }
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, logger);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger logger)
    {
        logger.LogError(ex, "");
        if (ex.InnerException is not null) logger.LogError(ex.InnerException.ToString());

        if (context.Response.HasStarted)
            return Task.CompletedTask;

        HttpStatusCode code;
        switch (ex)
        {
            case ArgumentException:
            case ValidationException:
                code = HttpStatusCode.BadRequest;
                break;
            case ForbiddenException:
                code = HttpStatusCode.Forbidden;
                break;
            case NotFoundException:
                code = HttpStatusCode.NotFound;
                break;
            default:
                code = HttpStatusCode.InternalServerError;
                break;
        }

        var result = JsonSerializer.Serialize(new { errorMessage = ex.Message });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}