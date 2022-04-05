using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OAuth.Sample.Domain.Helper;
using OAuth.Sample.Domain.Model.Line;
using OAuth.Sample.Domain.Shared;
using OAuth.Sample.Service.Interface;

namespace OAuth.Sample.Service.Service
{
    public class LineNotifyProvider : IOAuthProvider
    {
        public async Task<string> GetTokenAsync(OAuthSetting setting, string code)
        {
            var request = new RequestToken()
            {
                grant_type = "authorization_code",
                code = code,
                redirect_uri = setting.RedirectUri,
                client_id = setting.ClientId,
                client_secret = setting.ClientSecret
            };
            var result = await HttpClientHelper.PostAsync<ResponseToken>("https://notify-bot.line.me/oauth/token", request);
            if (result.StatusCode != HttpStatusCode.OK.ToInt()) throw new Exception("LineNotify Get TokenAsync Failed");

            return result.Data?.access_token;
        }

        public Task<UserProfileData> GetProfileAsync(string accessToken)
        {
            throw new NotImplementedException();
        }

        public async Task RevokeAsync(OAuthSetting setting, string accessToken)
        {
            var result = await HttpClientHelper.PostAsync<object>("https://notify-api.line.me/api/revoke", null,
                customHeader: new Dictionary<string, string>() { { "Authorization", $"Bearer {accessToken}" } });

            if (result.StatusCode != HttpStatusCode.OK.ToInt()) throw new Exception("LineNotify Revoke Fail");
        }

        public async Task SendMessage(string accessToken, string Message)
        {
            var result = await HttpClientHelper.PostAsync<object>("https://notify-api.line.me/api/notify", new
                {
                    message = Message
                },
                customHeader: new Dictionary<string, string>() { { "Authorization", $"Bearer {accessToken}" } });

            if (result.StatusCode != HttpStatusCode.OK.ToInt()) throw new Exception("LineNotify Notify Fail");
        }

    }
}
