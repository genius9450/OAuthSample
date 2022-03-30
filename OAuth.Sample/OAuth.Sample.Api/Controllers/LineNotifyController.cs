using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAuth.Sample.Domain.Enum;
using OAuth.Sample.Domain.Model.Line;
using OAuth.Sample.Domain.Shared;
using OAuth.Sample.EF.Entity;
using OAuth.Sample.Service.Interface;
using OAuth.Sample.Service.Service;

namespace OAuth.Sample.Api.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class LineNotifyController : CommonController
    {
        private readonly IOAuthService _oAuthService;
        private readonly IBaseService _baseService;

        public LineNotifyController(IOAuthService oAuthService, IBaseService baseService)
        {
            _oAuthService = oAuthService;
            _baseService = baseService;
        }

        /// <summary>
        /// 訂閱
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Subscribe")]
        public async Task<ActionResult> Subscribe(RequestLineNotify input)
        {
            if (!UserId.HasValue) throw new Exception("User Not Exist");
            CheckUserExist(UserId.Value);

            var targetSetting = GetLineNotifySetting();

            var accessToken = await _oAuthService.GetAccessTokenAsync(targetSetting, input.Code);

            await HandleUserSetting(input, accessToken);

            return Ok();
        }

        /// <summary>
        /// 取消訂閱
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("UnSubscribe")]
        public async Task<ActionResult> UnSubscribe()
        {
            if (!UserId.HasValue) throw new Exception("User Not Exist");

            var targetSetting = GetLineNotifySetting();

            var notify = _baseService.GetSingle<UserOAuthSetting>(x =>
                x.UserId == UserId.Value && x.ProviderType == ProviderType.LineNotify.ToString());
            if (notify == null) return NoContent();

            await _oAuthService.RevokeAsync(targetSetting, notify.Key);
            await _baseService.DeleteAsync<UserOAuthSetting>(notify.Id);

            return Ok();
        }

        /// <summary>
        /// 群組發送通知
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Notify")]
        public async Task<ActionResult> Notify(RequestSendMessage input)
        {
            var notifies = _baseService.GetList<UserOAuthSetting>(x =>
                x.ProviderType == ProviderType.LineNotify.ToString());
            if (!notifies.Any()) return NoContent();

            foreach (var notify in notifies)
            {
                var provider = new LineNotifyProvider();
                await provider.SendMessage(notify.Key, input.Message);
            }

            return Ok();
        }

        private async Task HandleUserSetting(RequestLineNotify input, string accessToken)
        {
            if (!UserId.HasValue) throw new Exception("User Not Exist");

            var setting = _baseService.GetSingle<UserOAuthSetting>(x =>
                x.UserId == UserId.Value && x.ProviderType == ProviderType.LineNotify.ToString());
            if (setting == null)
            {
                await _baseService.CreateAsync(new UserOAuthSetting()
                {
                    UserId = UserId.Value,
                    ProviderType = ProviderType.LineNotify.ToString(),
                    Key = accessToken
                });
            }
            else
            {
                setting.Key = accessToken;
                await _baseService.UpdateAsync(setting);
            }
        }

        private static OAuthSetting? GetLineNotifySetting()
        {
            var targetSetting = Const.OAuthSettings.FirstOrDefault(x => x.ProviderType == ProviderType.LineNotify);
            if (targetSetting == null) throw new Exception($"{ProviderType.LineNotify} Setting Not Found");
            return targetSetting;
        }
        private void CheckUserExist(int userId)
        {
            var user = _baseService.GetSingle<User>(x => x.Id == userId);
            if (user == null) throw new Exception("User Not Found");
        }
    }
}

