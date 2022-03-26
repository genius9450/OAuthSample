using System.Threading.Tasks;
using OAuth.Sample.Domain.Model.Line;
using OAuth.Sample.Domain.Shared;
using OAuth.Sample.Service.Service;

namespace OAuth.Sample.Service.Interface
{
    public interface IOAuthProvider
    {

        Task<string> GetTokenAsync(OAuthSetting setting, string code);

        Task<UserProfileData> GetProfileAsync(string accessToken);

        Task RevokeAsync(string accessToken);
    }
}