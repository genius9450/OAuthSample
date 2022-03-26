//using Altob.PaymentService.Domain.Model.Shared;

using OAuth.Sample.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace OAuth.Sample.Api
{
    public static class Const
    {
        /// <summary>
        /// 環境名稱
        /// </summary>
        public static string EnvironmentName { get; set; }

        /// <summary>
        /// 預設資料庫連線
        /// </summary>
        public static string DefaultConnectionString { get; set; }

        /// <summary>
        /// Logger
        /// </summary>
        public static ILogger<Startup> Logger { get; set; }

        public static OAuthSetting OAuthSetting { get; set; }

    }
}

