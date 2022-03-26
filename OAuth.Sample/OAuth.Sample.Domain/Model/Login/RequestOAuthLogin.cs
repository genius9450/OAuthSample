using OAuth.Sample.Domain.Enum;

namespace OAuth.Sample.Domain.Model.Login
{
    public class RequestOAuthLogin
    {
        public ProviderType ProviderType { get; set; }
        public string Code { get; set; }
    }
}
