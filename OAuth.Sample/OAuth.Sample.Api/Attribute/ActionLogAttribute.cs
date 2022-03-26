using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using OAuth.Sample.Domain.Shared;

namespace OAuth.Sample.Api.Attribute
{
    public class ActionLogAttribute : ActionFilterAttribute
    {
        private ILogger<ActionLogAttribute> logger;
        private ApiLogModel apiLogModel { get; set; }

        private Stopwatch stopWatch { get; set; }

        public ActionLogAttribute(ILogger<ActionLogAttribute> _logger)
        {
            logger = _logger;
            apiLogModel = new ApiLogModel();
            stopWatch = new Stopwatch();
        }

        /// <summary>
        /// 動作執行前
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            stopWatch.Start();
            apiLogModel.HttpMethod = context.HttpContext.Request.Method;
            apiLogModel.ClientIP = context.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            apiLogModel.FullPath = $"{context.HttpContext.Request.Host}{context.HttpContext.Request.Path.Value}";

            var Data = context.ActionArguments;
            apiLogModel.Request = Data == null ? "" : JsonConvert.SerializeObject(Data);
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// 動作執行後
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            string responseString;
            if (context.Result == null) responseString = "";
            else if (context.Result.GetType().Name == "ObjectResult")
            {
                responseString = JsonConvert.SerializeObject(((ObjectResult)context.Result).Value);
            }
            else
            {
                responseString = JsonConvert.SerializeObject(context.Result);
            }
            apiLogModel.Response = responseString.Length >= 1000 ? $"{responseString.Substring(0, 1000)} ...etc" : responseString;

            stopWatch.Stop();
            logger.LogInformation("{HttpMethod} / {FullPath} / {Request} / {Response} / {ClientIP} / {SpendSeconds}", apiLogModel.HttpMethod, apiLogModel.FullPath, apiLogModel.Request, apiLogModel.Response, apiLogModel.ClientIP, stopWatch.Elapsed.TotalSeconds);

            await base.OnResultExecutionAsync(context, next);
        }

    }

}

