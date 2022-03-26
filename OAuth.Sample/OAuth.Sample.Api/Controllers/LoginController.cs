using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAuth.Sample.Domain.Enum;
using OAuth.Sample.Domain.Helper;
using OAuth.Sample.Domain.Model.Line;
using OAuth.Sample.Domain.Model.Login;
using OAuth.Sample.Domain.Model.User;
using OAuth.Sample.Service.Interface;
using OAuth.Sample.Service.Service;

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

        public LoginController(IOAuthService oAuthService, IUserService userService)
        {
            _oAuthService = oAuthService;
            _userService = userService;
        }


        [HttpPost("OAuthLogin")]
        public async Task<LoginResponse> OAuthLogin(RequestOAuthLogin input)
        {
            var targetSetting = Const.OAuthSettings.FirstOrDefault(x => x.ProviderType == input.ProviderType);
            if (targetSetting == null) throw new Exception($"{input.ProviderType} Setting Not Found");

            var profile = await _oAuthService.GetProfileAsync(targetSetting, input.Code);

            return await _userService.LoginAsync(new LoginRequest()
            {
                Name = profile.Name,
                PhotoUrl = profile.PhotoUrl,
                Description = profile.Description,
                Key = profile.UserKey,
                ProviderType = nameof(ProviderType.LineLogin)
            });
        }

    }
}

