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
        private readonly ILineService _lineService;
        private readonly IUserService _userService;

        public LoginController(ILineService lineService, IUserService userService)
        {
            _lineService = lineService;
            _userService = userService;
        }


        [HttpPost("OAuthLogin")]
        public async Task<LoginResponse> OAuthLogin(RequestOAuthLogin input)
        {
            var accessToken = await _lineService.TokenAsync(Const.OAuthSetting, input.Code);
            var profile = await _lineService.GetProfileAsync(accessToken);

            return await _userService.LoginAsync(new LoginRequest()
            {
                Name = profile.displayName,
                PhotoUrl = profile.pictureUrl,
                Description = profile.statusMessage,
                Key = profile.userId,
                ProviderType = nameof(ProviderType.LineLogin)
            });
        }

    }
}

