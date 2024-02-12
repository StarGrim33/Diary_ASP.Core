using FonTech.Domain.Result;
using System.Net;
using ILogger = Serilog.ILogger;

namespace FonTech.Api.Middlewares
{
    /// <summary>
    /// Глобальная обработка ошибок
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger _logger;
        
        public ExceptionHandlingMiddleware(RequestDelegate deleagte, ILogger logger)
        {
            _requestDelegate = deleagte;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            _logger.Error(exception: exception, exception.Message);
            var errorMessage = exception.Message;

            var response = exception switch
            {
                BadHttpRequestException _ => new BaseResult() { ErrorMessage = errorMessage, ErrorCode = (int) HttpStatusCode.BadRequest},
                UnauthorizedAccessException _ => new BaseResult() { ErrorMessage = errorMessage, ErrorCode = (int) HttpStatusCode.Unauthorized},
                ArgumentNullException _ => new BaseResult() { ErrorMessage = errorMessage, ErrorCode= (int) HttpStatusCode.BadRequest},
                _ => new BaseResult() { ErrorMessage = "Internal server error. Please retry later", ErrorCode = (int) HttpStatusCode.InternalServerError},
            };

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = response.ErrorCode;
            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }
}
