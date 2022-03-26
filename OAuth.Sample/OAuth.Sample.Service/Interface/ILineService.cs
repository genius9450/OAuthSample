using System.Threading.Tasks;
using OAuth.Sample.Domain.Model.Line;
using OAuth.Sample.Domain.Shared;

namespace OAuth.Sample.Service.Interface
{
    public interface ILineService
    {
        Task<string> TokenAsync(OAuthSetting setting, string code);

        Task<ProfileModel> GetProfileAsync(string accessToken);
    }
}