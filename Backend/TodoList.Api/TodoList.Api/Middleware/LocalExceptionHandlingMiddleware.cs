using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UuidExtensions;

namespace TodoList.Api.Middleware;

public class LocalExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<LocalExceptionHandlingMiddleware> _logger;

    public LocalExceptionHandlingMiddleware(ILogger<LocalExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // TODO: get trace id from dotnet.. not a Guid...
        var transactionId = Uuid7.Guid();

        using (_logger.BeginScope(new List<KeyValuePair<string, object>>
        {
            new KeyValuePair<string, object>("TransactionId", transactionId),
        }))

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // Not a good example of Semantic Logging...
            _logger.LogError("Error {Message} {Exception}", ex.Message, ex);

            await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
        }
    }
}