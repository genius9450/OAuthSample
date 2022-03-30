using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAuth.Sample.Domain.Model.User;
using OAuth.Sample.Service.Interface;

namespace OAuth.Sample.Api.Controllers
{
    /// <summary>
    /// 使用者
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UserController : CommonController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserData>> Get(int id)
        {
            return await _userService.GetUserAsync(id);
        }

        [Authorize]
        [HttpGet("GetSelf")]
        public async Task<ActionResult<UserData>> GetSelf()
        {
            if (!UserId.HasValue) throw new Exception("User Not Exist");
            return await _userService.GetUserAsync(UserId.Value);
        }

    }
}

