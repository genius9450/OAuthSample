using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IConfiguration _configuration;

        public UserService(IBaseService baseService, IConfiguration configuration)
        {
            _baseService = baseService;
            _configuration = configuration;
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
                        ProviderType = input.ProviderType.ToString(),
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

    }
}
