using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth.Sample.Domain.Enum
{
    public enum ProviderType
    {
        [Description("Line")]
        LineLogin,
        [Description("Line通知")]
        LineNotify,
        [Description("Facebook")]
        FacebookLogin,
        GoogleLogin
    }
}
