using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OAuth.Sample.Domain.Shared;

namespace OAuth.Sample.Domain.Model.User
{
    public class UserData
    {

        public int UserId { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        
        public List<UserOAuthData> UserOAuthData { get; set; }

    }

    public class SelfUserData: UserData
    {
        public string Account { get; set; }
    }

    public class UserOAuthData
    {
        /// <summary>
        /// Resource Server Type
        /// </summary>
        /// <remarks>
        /// ref: Enum: ProviderType
        /// Line Login、Line Notify、Google、Facebook...
        /// </remarks>
        public string ProviderType { get; set; }

        /// <summary>
        /// 啟用時間
        /// </summary>
        public string ActiveDateTime { get; set; }

    }
}
