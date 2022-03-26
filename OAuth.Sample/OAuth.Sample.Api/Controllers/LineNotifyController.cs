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
using Microsoft.VisualBasic;
using OAuth.Sample.Domain.Enum;
using OAuth.Sample.Domain.Helper;
using OAuth.Sample.Domain.Model.Line;
using OAuth.Sample.Domain.Model.Login;
using OAuth.Sample.Domain.Model.User;
using OAuth.Sample.Domain.Shared;
using OAuth.Sample.EF.Entity;
using OAuth.Sample.Service.Interface;
using OAuth.Sample.Service.Service;

namespace OAuth.Sample.Api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class LineNotifyController : ControllerBase
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
        public async Task Subscribe(RequestLineNotify input)
        {
            CheckUserExist(input);

            var targetSetting = GetLineNotifySetting();

            var accessToken = await _oAuthService.GetAccessTokenAsync(targetSetting, input.Code);

            await HandleUserSetting(input, accessToken);
        }

        /// <summary>
        /// 取消訂閱
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("UnSubscribe")]
        public async Task UnSubscribe(RequestLineNotify input)
        {
            var targetSetting = GetLineNotifySetting();

            var notify = _baseService.GetSingle<UserOAuthSetting>(x =>
                x.UserId == input.UserId && x.ProviderType == ProviderType.LineNotify.ToString());

            await _oAuthService.RevokeAsync(targetSetting, notify.Key);
            await _baseService.DeleteAsync<UserOAuthSetting>(notify);
        }

        /// <summary>
        /// 群組發送通知
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Notify")]
        public async Task Notify(string Message)
        {
            var notifies = _baseService.GetList<UserOAuthSetting>(x =>
                x.ProviderType == ProviderType.LineNotify.ToString());
            if (!notifies.Any()) return;

            foreach (var notify in notifies)
            {
                var provider = new LineNotifyProvider();
                await provider.SendMessage(notify.Key, Message);
            }
        }

        private async Task HandleUserSetting(RequestLineNotify input, string accessToken)
        {
            var setting = _baseService.GetSingle<UserOAuthSetting>(x =>
                x.UserId == input.UserId && x.ProviderType == ProviderType.LineNotify.ToString());
            if (setting == null)
            {
                await _baseService.CreateAsync(new UserOAuthSetting()
                {
                    UserId = input.UserId,
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
        private void CheckUserExist(RequestLineNotify input)
        {
            var user = _baseService.GetSingle<User>(x => x.Id == input.UserId);
            if (user == null) throw new Exception("User Not Found");
        }
    }

    public class RequestSendMessage
    {
        public int UserId { get; set; }

        public string Message { get; set; }

    }

    public class RequestLineNotify
    {
        public int UserId { get; set; }
        public string Code { get; set; }
    }
}

