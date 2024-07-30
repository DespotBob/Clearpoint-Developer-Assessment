using Microsoft.AspNetCore.Http;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace TodoList.Api.Middleware
{
    public class LocalExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<LocalExceptionHandlingMiddleware> _logger;

        public LocalExceptionHandlingMiddleware(ILogger<LocalExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
            }
        }
    }
}
