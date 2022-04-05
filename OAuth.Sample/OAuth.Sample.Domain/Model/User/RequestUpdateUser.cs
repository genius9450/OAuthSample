namespace OAuth.Sample.Domain.Model.User
{
    public class RequestUpdateUser
    {
        /// <summary>
        /// 使用者姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 登入帳號
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 登入密碼
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 電子信箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 使用者自我介紹
        /// </summary>
        public string Description { get; set; }

    }
}