using OAuth.Sample.Domain.Enum;

namespace OAuth.Sample.Domain.Model.Login
{
    public class RequestOAuthLoginConnect
    {
        public ProviderType ProviderType { get; set; }
        public string Code { get; set; }
        public string RedirectUri { get; set; }

    }
}