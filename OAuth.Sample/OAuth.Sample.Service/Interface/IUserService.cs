using System.Threading.Tasks;
using OAuth.Sample.Domain.Model.Line;
using OAuth.Sample.Domain.Model.User;
using OAuth.Sample.EF.Entity;

namespace OAuth.Sample.Service.Interface
{
    public interface IUserService
    {
        Task<LoginResponse> LoginAsync(LoginRequest profile);
        Task<UserData> GetUserAsync(int userId);
    }
}