using System;
using System.Collections.Generic;
using System.Text;

namespace OAuth.Sample.Domain.Shared
{
    /// <summary>
    /// API Log 紀錄
    /// </summary>
    public class ApiLogModel
    {
        /// <summary>
        /// Http Method
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// API完整路徑
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 請求
        /// </summary>
        public string Request { get; set; }

        /// <summary>
        /// 回傳
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Clietn IP        
        /// </summary>
        public string ClientIP { get; set; }

    }
}

