using System.Threading.Tasks;
using OAuth.Sample.Domain.Model.Line;
using OAuth.Sample.Domain.Model.User;
using OAuth.Sample.Domain.Shared;
using OAuth.Sample.Service.Service;

namespace OAuth.Sample.Service.Interface
{
    public interface IOAuthService
    {
        Task<UserProfileData> GetProfileAsync(OAuthSetting setting, string code);

        Task<string> GetAccessTokenAsync(OAuthSetting setting, string code);

        Task RevokeAsync(OAuthSetting setting, string accessToken, string key);
    }
}