using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OAuth.Sample.EF.Entity
{
    [Table("UserOAuthSetting")]
    public class UserOAuthSetting : BaseEntity
    {

        /// <summary>
        /// 使用者Id
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Resource Server Type
        /// </summary>
        /// <remarks>
        /// ref: Enum: ProviderType
        /// Line Login、Line Notify、Google、Facebook...
        /// </remarks>
        [Required]
        [StringLength(20)]
        public string ProviderType { get; set; }

        public string AccessToken { get; set; }

        /// <summary>
        /// 識別值
        /// </summary>
        /// <remarks>
        /// Line Login: userId
        /// Facebook Login: userId
        /// Line Notify: access_token
        /// </remarks>
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// 啟用時間
        /// </summary>
        [Required]
        public DateTime ActiveDateTime { get; set; }

    }
}
