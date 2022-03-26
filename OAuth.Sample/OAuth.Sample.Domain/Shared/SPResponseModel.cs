namespace OAuth.Sample.Domain.Shared
{
    /// <summary>
    /// SP回傳結果
    /// </summary>
    public class SPResponseModel<T>
    {
        /// <summary>
        /// 執行結果(0:成功; 1:失敗)
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// 訊息
        /// </summary>
        public string ErrMessage { get; set; }

        /// <summary>
        /// 回傳資料
        /// </summary>
        public T Data { get; set; }
    }
}

