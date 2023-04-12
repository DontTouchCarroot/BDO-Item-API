using ItemBase.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace ItemBase.Presentation.Middleware
{
    public class ExceptionMiddleware : IMiddleware
    {

        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            } catch (NotFoundException ex)
            {
                await HandleNotFouncException(ex, context);
            } catch (AlreadyExsisttException ex)
            {
                await HandleAlreadyExsisttException(ex, context);
            } catch (Exception ex)
            {
                await HandleOtherException(ex, context);
            }

        }
        private async Task HandleNotFouncException(NotFoundException exception, HttpContext httpContext)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

            var problem = new
            {
                message = exception.Message,
            };

            var json = JsonSerializer.Serialize(problem);

            await httpContext.Response.WriteAsync(json);
        }


        private async Task HandleAlreadyExsisttException(AlreadyExsisttException exception, HttpContext httpContext)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var problem = new
            {
                message = exception.Message,
            };

            var json = JsonSerializer.Serialize(problem);

            await httpContext.Response.WriteAsync(json);
        }
        private async Task HandleOtherException(Exception exception, HttpContext httpContext)
        {
            _logger.LogCritical(exception.Message);

            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var problem = new
            {
                Type = "Server Error",
                Title = "Server Error",
                Detail = "An internal server has occurred"
            };

            var json = JsonSerializer.Serialize(problem);

            await httpContext.Response.WriteAsync(json);

        }
    }
}
