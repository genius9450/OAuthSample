using System;
using System.Collections.Generic;
using System.Text;

namespace OAuth.Sample.Domain.Shared
{
    public class HttpResponseModel<TResponse>
    {
        /// <summary>
        /// 狀態碼
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// 訊息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 回傳資料
        /// </summary>
        public TResponse Data { get; set; }

    }
}

