using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OAuth.Sample.Api.Helper;
using OAuth.Sample.Domain.Model.Login;
using OAuth.Sample.Domain.Model.User;
using OAuth.Sample.Service.Interface;

namespace OAuth.Sample.Api.Controllers
{
    /// <summary>
    /// Login
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IOAuthService _oAuthService;
        private readonly IUserService _userService;
        private readonly JwtHelpers _jwt;

        public LoginController(IOAuthService oAuthService, IUserService userService, JwtHelpers jwt)
        {
            _oAuthService = oAuthService;
            _userService = userService;
            _jwt = jwt;
        }

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
                ProviderType = input.ProviderType
            });
        }

    }
}

