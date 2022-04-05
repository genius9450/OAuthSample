using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OAuth.Sample.Domain.Enum;
using OAuth.Sample.Domain.Helper;
using OAuth.Sample.Domain.Model.Line;
using OAuth.Sample.Domain.Model.User;
using OAuth.Sample.EF;
using OAuth.Sample.EF.Entity;
using OAuth.Sample.Service.Interface;

namespace OAuth.Sample.Service.Service
{
    public class UserService : IUserService
    {
        private readonly IBaseService _baseService;
        private readonly IConfiguration _configuration;
        private readonly OAuthSampleDBContext _dbContext;

        public UserService(IBaseService baseService, IConfiguration configuration, OAuthSampleDBContext dbContext)
        {
            _baseService = baseService;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest input)
        {
            var userOAuthSetting = _baseService.GetSingle<UserOAuthSetting>(x => x.ProviderType == input.ProviderType.ToString() && x.Key == input.Key);
            var user = userOAuthSetting == null
                ? await CreteUserAsync(input)
                : await UpdateUserAsync(input, userOAuthSetting);

            return new LoginResponse()
            {
                UserId = user.Id,
                JwtToken = GenerateJwtToken(user.Id)
            };
        }

        public async Task<SelfUserData> GetUserAsync(int userId)
        {
            var users = await GetUserWithOAuthSetting(userId);
            if (!users.Any()) throw new Exception("User Not Found");

            var user = users.FirstOrDefault();
            return new SelfUserData()
            {
                UserId = user.Id,
                Account = user.Account,
                Name = user.Name,
                Description = user.Description,
                Email = user.Email,
                PhotoUrl = user.PhotoUrl,
                UserOAuthData = user.UserOAuthSettings.Select(x => new UserOAuthData() { ProviderType = x.ProviderType, ActiveDateTime = x.ActiveDateTime.ToString("yyyy/MM/dd HH:mm") }).ToList()
            };
        }

        public async Task<List<UserData>> GetUserListAsync()
        {
            var users = await GetUserWithOAuthSetting(null);
            return users.Select(user => new UserData()
            {
                UserId = user.Id,
                Name = user.Name,
                Description = user.Description,
                Email = user.Email,
                PhotoUrl = user.PhotoUrl,
                UserOAuthData = user.UserOAuthSettings.Select(x => new UserOAuthData() { ProviderType = x.ProviderType, ActiveDateTime = x.ActiveDateTime.ToString("yyyy/MM/dd HH:mm") }).ToList()
            }).ToList();
        }

        private async Task<List<User>> GetUserWithOAuthSetting(int? userId)
        {
            var query = _dbContext.GetDbSet<User>()
                .Include(x => x.UserOAuthSettings)
                .Where(x => !userId.HasValue || x.Id == userId.Value);
            return await query.ToListAsync();
        }

        private async Task<User> CreteUserAsync(LoginRequest input)
        {
            var user = await _baseService.CreateAsync(new User()
            {
                Name = input.Name,
                PhotoUrl = input.PhotoUrl,
                Email = input.Email,
                Description = input.Description,
                Password = "********",
                UserOAuthSettings = new List<UserOAuthSetting>()
                {
                    new UserOAuthSetting()
                    {
                        AccessToken = input.AccessToken,
                        ProviderType = input.ProviderType.ToString(),
                        Key = input.Key,
                        CreateDateTime = DateTime.Now,
                        ActiveDateTime = DateTime.Now
                    }
                },
                CreateDateTime = DateTime.Now
            });
            return user;
        }

        private async Task<User> UpdateUserAsync(LoginRequest input, UserOAuthSetting userOAuthSetting)
        {
            var user = _baseService.GetSingle<User>(x => x.Id == userOAuthSetting.UserId);
            if (user == null) throw new Exception("User Not Found");

            user.PhotoUrl = input.PhotoUrl;
            await _baseService.UpdateAsync(user);
            return user;
        }

        public string GenerateJwtToken(int userId)
        {
            var issuer = _configuration.GetValue<string>("JwtSettings:Issuer");
            var signKey = _configuration.GetValue<string>("JwtSettings:SignKey");

            // Configuring "Claims" to your JWT Token
            var claims = new List<Claim>();

            // In RFC 7519 (Section#4), there are defined 7 built-in Claims, but we mostly use 2 of them.
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())); // User.Identity.Name
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // JWT ID
            var userClaimsIdentity = new ClaimsIdentity(claims);

            // Create a SymmetricSecurityKey for JWT Token signatures
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));

            // HmacSha256 MUST be larger than 128 bits, so the key can't be too short. At least 16 and more characters.
            // https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // Create SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Subject = userClaimsIdentity,
                Expires = DateTime.Now.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpiresMin")),
                SigningCredentials = signingCredentials
            };

            // Generate a JWT securityToken, than get the serialized Token result (string)
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }

        public async Task UserOAuthLoginConnectAsync(int userId, ProviderType providerType, string userKey,
            string accessToken)
        {
            var user = _baseService.GetSingle<User>(x => x.Id == userId);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }

            var userOAuthSettings = _baseService.GetList<UserOAuthSetting>(x =>
                x.Key == userKey && x.ProviderType == providerType.ToString());
            if (userOAuthSettings.Any(x => x.UserId != userId))
            {
                throw new Exception($"此{providerType.Description()}帳號，已被其他使用者綁定");
            }

            var existOAuthSetting = userOAuthSettings.SingleOrDefault(x => x.UserId == userId);
            if (existOAuthSetting != null)
            {
                // 已綁定，僅更新異動時間
                existOAuthSetting.ModifyDateTime = DateTime.Now;
                existOAuthSetting.Key = userKey;
                existOAuthSetting.AccessToken = accessToken;
                await _baseService.UpdateAsync(existOAuthSetting);
                return;
            }


            // 未綁定，綁定此OAuth Login設定
            await _baseService.CreateAsync<UserOAuthSetting>(new UserOAuthSetting()
            {
                UserId = userId,
                CreateDateTime = DateTime.Now,
                Key = userKey,
                AccessToken = accessToken,
                ProviderType = providerType.ToString(),
                ActiveDateTime = DateTime.Now
            });

        }

        public async Task UpdateUserAsync(int userId, RequestUpdateUser input)
        {
            var user = _baseService.GetSingle<User>(x => x.Id == userId);
            if (user == null) throw new Exception("User Not Found");

            if (!string.IsNullOrWhiteSpace(input.Account))
            {
                var theSameAccount = _baseService.GetList<User>(x => x.Account == input.Account && x.Id != userId);
                if (theSameAccount.Any())
                {
                    throw new Exception("此登入帳號已被註冊");
                }
            }

            user.Name = input.Name;
            user.Account = input.Account;
            user.Email = input.Email;
            user.Description = input.Description;
            if (input.Password != "********") user.Password = input.Password;
            await _baseService.UpdateAsync(user);
        }
    }
}
