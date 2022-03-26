using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OAuth.Sample.Domain.Helper;
using OAuth.Sample.Domain.Model.Line;
using OAuth.Sample.Domain.Model.User;
using OAuth.Sample.EF.Entity;
using OAuth.Sample.Service.Interface;

namespace OAuth.Sample.Service.Service
{
    public class UserService : IUserService
    {
        private readonly IBaseService _baseService;

        public UserService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest input)
        {
            var userOAuthSetting = _baseService.GetSingle<UserOAuthSetting>(x => x.ProviderType == input.ProviderType && x.Key == input.Key);
            var user = userOAuthSetting == null
                ? await CreteUserAsync(input)
                : await UpdateUserAsync(input, userOAuthSetting);

            return new LoginResponse()
            {
                UserId = user.Id,
                //Name = user.Name,
                //Description = user.Description,
                //PhotoUrl = user.PhotoUrl
            };
        }

        public async Task<UserData> GetUserAsync(int userId)
        {
            var user = _baseService.GetSingle<User>(x=> x.Id == userId);
            if (user == null) throw new Exception("User Not Found");

            return new UserData()
            {
                UserId = user.Id,
                Name = user.Name,
                Description = user.Description,
                PhotoUrl = user.PhotoUrl
            };
        }

        private async Task<User> CreteUserAsync(LoginRequest input)
        {
            var user = await _baseService.CreateAsync(new User()
            {
                Name = input.Name,
                PhotoUrl = input.PhotoUrl,
                Description = input.Description,
                UserOAuthSettings = new List<UserOAuthSetting>()
                {
                    new UserOAuthSetting()
                    {
                        ProviderType = input.ProviderType,
                        Key = input.Key
                    }
                }
            });
            return user;
        }

        private async Task<User> UpdateUserAsync(LoginRequest input, UserOAuthSetting userOAuthSetting)
        {
            var user = _baseService.GetSingle<User>(x => x.Id == userOAuthSetting.UserId);
            if (user == null) throw new Exception("User Not Found");

            user.Name = input.Name;
            user.PhotoUrl = input.PhotoUrl;
            user.Description = input.Description;
            await _baseService.UpdateAsync(user);
            return user;
        }
    }
}
