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
    public class GoogleLoginProvider : IOAuthProvider
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
            var result = await HttpClientHelper.PostAsync<ResponseToken>("https://oauth2.googleapis.com/token", request);
            if (result.StatusCode != HttpStatusCode.OK.ToInt()) throw new Exception("Google Get Token Failed");

            return result.Data?.access_token;
        }

        public async Task<UserProfileData> GetProfileAsync(string accessToken)
        {
            var result = await HttpClientHelper.GetAsync<object>("https://www.googleapis.com/drive/v2/files", new { access_token = accessToken });
            if (result.StatusCode != HttpStatusCode.OK.ToInt()) throw new Exception("Google Get Profile Failed");

            // TODO: 403 Forbidden
            return new UserProfileData()
            {
                AccessToken = accessToken,
                //Name = result.Data.name,
                //PhotoUrl = result.Data.picture?.data?.url,
                //Description = result.Data.email,
                //UserKey = result.Data.id
            };
        }

        public Task RevokeAsync(OAuthSetting setting, string accessToken)
        {
            throw new NotImplementedException();
        }
    }
}
