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
    public class UserController : ControllerBase
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
            var principal = HttpContext.User;
            var userId = principal?.Claims?.SingleOrDefault(p => p.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            return await _userService.GetUserAsync(int.Parse(userId));
        }

    }
}

