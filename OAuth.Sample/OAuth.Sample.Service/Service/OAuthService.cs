using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using OAuth.Sample.Domain.Helper;
using OAuth.Sample.Domain.Model.Line;
using OAuth.Sample.Domain.Model.User;
using OAuth.Sample.Domain.Shared;
using OAuth.Sample.Service.Interface;

namespace OAuth.Sample.Service.Service
{
    public class OAuthService : IOAuthService
    {
        public async Task<string> GetAccessTokenAsync(OAuthSetting setting, string code)
        {
            var provider = GetProvider(setting);
            return await provider.GetTokenAsync(setting, code);
        }

        public async Task RevokeAsync(OAuthSetting setting, string accessToken)
        {
            var provider = GetProvider(setting);
            await provider.RevokeAsync(accessToken);
        }

        public async Task<UserProfileData> GetProfileAsync(OAuthSetting setting, string code)
        {
            var provider = GetProvider(setting);

            var accessToken = await provider.GetTokenAsync(setting, code);
            return await provider.GetProfileAsync(accessToken);
        }

        private static IOAuthProvider GetProvider(OAuthSetting setting)
        {
            IOAuthProvider provider;
            try
            {
                Type t = Type.GetType($"OAuth.Sample.Service.Service.{setting.ProviderType}Provider");
                provider = (IOAuthProvider)Activator.CreateInstance(t);
            }
            catch (Exception e)
            {
                throw new Exception($"Unknown Provider:{setting.ProviderType}");
            }

            return provider;
        }
    }

    public class UserProfileData
    {
        public string UserKey { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string Description { get; set; }
    }
}
