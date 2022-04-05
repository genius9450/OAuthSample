using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAuth.Sample.Domain.Model.User;
using OAuth.Sample.EF.Entity;
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
        private readonly IBaseService _baseService;

        public UserController(IUserService userService, IBaseService baseService)
        {
            _userService = userService;
            _baseService = baseService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserData>> Get(int id)
        {
            return await _userService.GetUserAsync(id);
        }

        /// <summary>
        /// 取得當前登入者資訊
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetSelf")]
        public async Task<ActionResult<SelfUserData>> GetSelf()
        {
            if (!UserId.HasValue) throw new Exception("User Not Exist");
            return await _userService.GetUserAsync(UserId.Value);
        }

        /// <summary>
        /// 取得使用者列表
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetUserList")]
        public async Task<ActionResult<List<UserData>>> GetUserList()
        {
            return await _userService.GetUserListAsync();
        }

        /// <summary>
        /// 編輯使用者
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("Update")]
        public async Task<ActionResult> UpdateUser(RequestUpdateUser input)
        {
            if (!UserId.HasValue) throw new Exception("User Not Exist");
            await _userService.UpdateUserAsync(UserId.Value, input);
            return Ok();
        }

    }
}

