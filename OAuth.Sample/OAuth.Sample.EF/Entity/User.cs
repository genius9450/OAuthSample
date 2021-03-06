using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OAuth.Sample.EF.Entity
{
    [Table("User")]
    public class User : BaseEntity
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
        /// 登入密碼(暫時不做...)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 使用者照片URL
        /// </summary>
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 電子信箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 使用者自我介紹
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 使用者OAuth設定
        /// </summary>
        [ForeignKey("UserId")]
        public ICollection<UserOAuthSetting> UserOAuthSettings { get; set; }

    }
}
