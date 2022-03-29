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
    public class FacebookLoginProvider : IOAuthProvider
    {
        public async Task<string> GetTokenAsync(OAuthSetting setting, string code)
        {
            var request = new RequestToken()
            {
                code = code,
                redirect_uri = setting.RedirectUri,
                client_id = setting.ClientId,
                client_secret = setting.ClientSecret
            };
            var result = await HttpClientHelper.GetAsync<ResponseToken>("https://graph.facebook.com/v13.0/oauth/access_token", request);
            if (result.StatusCode != HttpStatusCode.OK.ToInt()) throw new Exception("Facebook Get Token Failed");

            return result.Data?.access_token;
        }

        public async Task<UserProfileData> GetProfileAsync(string accessToken)
        {
            var request = new RquestFacebookProfile
            {
                access_token = accessToken,
                fields = "id,email,name,picture"
            };
            var result = await HttpClientHelper.GetAsync<ResponseFacebookProfile>("https://graph.facebook.com/v13.0/me", request);
            if (result.StatusCode != HttpStatusCode.OK.ToInt()) throw new Exception("Facebook Get GetProfile Failed");

            return new UserProfileData()
            {
                Name = result.Data.name,
                PhotoUrl = result.Data.picture?.data?.url,
                Email = result.Data.email,
                UserKey = result.Data.id
            };
        }

        public Task RevokeAsync(string accessToken)
        {
            throw new NotImplementedException();
        }
    }
}
