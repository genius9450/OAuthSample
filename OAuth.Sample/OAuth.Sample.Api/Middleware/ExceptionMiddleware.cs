using OAuth.Sample.Api.Helper;
using OAuth.Sample.Domain.Enum;
using OAuth.Sample.Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace OAuth.Sample.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger<ExceptionMiddleware> logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> _logger)
        {
            logger = _logger;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                RecordExceptionAsync(context, ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// 記錄錯誤訊息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private void RecordExceptionAsync(HttpContext context, Exception ex)
        {            
            logger.LogError(ex, "{FullPath} / {ExceptionMessage} / {ClientIP}", context.Request.Path.Value, ex.Message, context.Connection.RemoteIpAddress.MapToIPv4().ToString());
        }

        /// <summary>
        /// 回傳錯誤狀態
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
#if DEBUG
            string message = ex.Message;
#else
            string message = "InternalServerError";
#endif
            var result = JsonConvert.SerializeObject(new ResponseModel<object>() { Msg = message, StatusCode = ResponseStatusCode.Fail.ToInt() });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = HttpStatusCode.InternalServerError.ToInt();
            return context.Response.WriteAsync(result);
        }
    }
}

