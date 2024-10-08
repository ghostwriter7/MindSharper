﻿using MindSharper.Domain.Exceptions;

namespace MindSharper.Presentation.API.Middlewares;

public class ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (UnauthorizedException unauthorizedException)
        {
            logger.LogWarning(unauthorizedException.Message);
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync(unauthorizedException.Message);
        }
        catch (DuplicateResourceException duplicateResourceException)
        {
            logger.LogWarning(duplicateResourceException.Message);
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(duplicateResourceException.Message);
        }
        catch (NotFoundException notFoundException)
        {
            logger.LogWarning(notFoundException.Message);
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync(notFoundException.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, exception.Message);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Something went wrong, apologies.");
        }
    }
}