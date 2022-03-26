using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OAuth.Sample.Domain.Enum
{
    /// <summary>
    /// 回傳狀態碼
    /// </summary>
    public enum ResponseStatusCode
    {
        [Description("成功")]
        Success = 200,

        [Description("失敗")]
        Fail = 400,
        [Description("參數錯誤")]
        ParameterError = 401,
    }

}

