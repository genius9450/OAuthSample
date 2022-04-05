using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAuth.Sample.Api.Helper;
using OAuth.Sample.Domain.Enum;
using OAuth.Sample.Domain.Model.Login;
using OAuth.Sample.Domain.Model.User;
using OAuth.Sample.EF.Entity;
using OAuth.Sample.Service.Interface;

namespace OAuth.Sample.Api.Controllers
{
    /// <summary>
    /// Login
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class LoginController : CommonController
    {
        private readonly IOAuthService _oAuthService;
        private readonly IUserService _userService;
        private readonly IBaseService _baseService;
        public LoginController(IOAuthService oAuthService, IUserService userService, IBaseService baseService)
        {
            _oAuthService = oAuthService;
            _userService = userService;
            _baseService = baseService;
        }

        /// <summary>
        /// 透過OAuth登入
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("OAuthLogin")]
        public async Task<ActionResult<LoginResponse>> OAuthLogin(RequestOAuthLogin input)
        {
            var targetSetting = Const.OAuthSettings.FirstOrDefault(x => x.ProviderType == input.ProviderType);
            if (targetSetting == null) return NotFound(); //throw new Exception($"{input.ProviderType} Setting Not Found");

            var profile = await _oAuthService.GetProfileAsync(targetSetting, input.Code);

            return await _userService.LoginAsync(new LoginRequest()
            {
                Name = profile.Name,
                PhotoUrl = profile.PhotoUrl,
                Description = profile.Description,
                Email = profile.Email,
                Key = profile.UserKey,
                AccessToken = profile.AccessToken,
                ProviderType = input.ProviderType
            });
        }

        /// <summary>
        /// 透過OAuth登入
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<ActionResult<LoginResponse>> Login(RequestLogin input)
        {
            var user = _baseService.GetSingle<User>(x => x.Account == input.Account && x.Password == input.Password);
            if (user == null) throw new Exception("帳號或密碼錯誤");

            return Ok(new LoginResponse()
            {
                UserId = user.Id,
                JwtToken = _userService.GenerateJwtToken(user.Id)
            });
        }

        /// <summary>
        /// 綁定OAuth登入
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Connect")]
        [Authorize]
        public async Task<ActionResult<object>> OAuthLoginConnect(RequestOAuthLoginConnect input)
        {
            var targetSetting = Const.OAuthSettings.FirstOrDefault(x => x.ProviderType == input.ProviderType);
            if (targetSetting == null) return NotFound(); //throw new Exception($"{input.ProviderType} Setting Not Found");
            if (!string.IsNullOrWhiteSpace(input.RedirectUri)) targetSetting.RedirectUri = input.RedirectUri;

            var profile = await _oAuthService.GetProfileAsync(targetSetting, input.Code);
            await _userService.UserOAuthLoginConnectAsync(UserId.Value, input.ProviderType, profile.UserKey, profile.AccessToken);
            return Ok(new ResponseOAuthLoginConnect() { Msg = "OAuth綁定成功" });
        }

        /// <summary>
        /// 解除綁定OAuth登入
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpDelete("Disconnect/{type}")]
        [Authorize]
        public async Task<ActionResult> OAuthLoginDisconnect(string type)
        {
            var provider = Enum.Parse(typeof(ProviderType), type);
            var targetSetting = Const.OAuthSettings.FirstOrDefault(x => x.ProviderType == (ProviderType)provider);
            if (targetSetting == null) return NotFound(); //throw new Exception($"{input.ProviderType} Setting Not Found");

            var oauth = _baseService.GetSingle<UserOAuthSetting>(x =>
                x.UserId == UserId.Value && x.ProviderType == provider.ToString());
            if (oauth == null) return NoContent();

            await _oAuthService.RevokeAsync(targetSetting, oauth.AccessToken, oauth.Key);
            await _baseService.DeleteAsync<UserOAuthSetting>(oauth.Id);

            return Ok();
        }


    }
}

