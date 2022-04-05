using System.Collections.Generic;
using System.Threading.Tasks;
using OAuth.Sample.Domain.Enum;
using OAuth.Sample.Domain.Model.Line;
using OAuth.Sample.Domain.Model.User;
using OAuth.Sample.EF.Entity;

namespace OAuth.Sample.Service.Interface
{
    public interface IUserService
    {
        Task<LoginResponse> LoginAsync(LoginRequest profile);
        Task<SelfUserData> GetUserAsync(int userId);
        Task<List<UserData>> GetUserListAsync();
        string GenerateJwtToken(int userId);

        Task UserOAuthLoginConnectAsync(int userId, ProviderType providerType, string userKey, string accessToken);

        Task UpdateUserAsync(int userId, RequestUpdateUser input);
    }
}