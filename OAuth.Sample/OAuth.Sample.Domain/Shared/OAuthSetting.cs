using OAuth.Sample.Domain.Enum;

namespace OAuth.Sample.Domain.Shared
{
    public class OAuthSetting
    {
        public ProviderType ProviderType { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }
    }
}