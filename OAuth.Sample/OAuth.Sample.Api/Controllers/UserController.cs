using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    /// 使用者
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [Authorize]
        [HttpGet("{id}")]
        public async Task<UserData> Get(int id)
        {
            return await _userService.GetUserAsync(id);
        }

    }
}

