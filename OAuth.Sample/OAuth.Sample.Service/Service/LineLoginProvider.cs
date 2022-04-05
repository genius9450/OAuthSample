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
   public class LineLoginProvider : IOAuthProvider
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
            var result = await HttpClientHelper.PostAsync<ResponseToken>("https://api.line.me/oauth2/v2.1/token", request);
            if (result.StatusCode != HttpStatusCode.OK.ToInt()) throw new Exception("Line Get TokenAsync Failed");

            return result.Data?.access_token;
        }

        public async Task<UserProfileData> GetProfileAsync(string accessToken)
        {
            var result = await HttpClientHelper.GetAsync<ProfileModel>("https://api.line.me/v2/profile", customHeader: new Dictionary<string, string>() { { "Authorization", $"Bearer {accessToken}" } });
            if (result.StatusCode != HttpStatusCode.OK.ToInt()) throw new Exception("Line Get GetProfileAsync Failed");

            return new UserProfileData()
            {
                AccessToken = accessToken,
                Name = result.Data.displayName,
                PhotoUrl = result.Data.pictureUrl,
                Description = result.Data.statusMessage,
                UserKey = result.Data.userId
            };
        }

        public async Task RevokeAsync(OAuthSetting setting, string accessToken)
        {
            var request = new
            {
                access_token = accessToken,
                client_id = setting.ClientId,
                client_secret = setting.ClientSecret
            };
            var result = await HttpClientHelper.PostAsync<object>("https://api.line.me/oauth2/v2.1/revoke", request);
            if (result.StatusCode != HttpStatusCode.OK.ToInt()) throw new Exception("Line Revoke Failed");
        }
    }
}
