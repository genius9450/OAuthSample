using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using OAuth.Sample.Domain.Helper;
using OAuth.Sample.Domain.Model.Line;
using OAuth.Sample.Domain.Shared;
using OAuth.Sample.Service.Interface;

namespace OAuth.Sample.Service.Service
{
    public class LineService : ILineService
    {

        public async Task<string> TokenAsync(OAuthSetting setting, string code)
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

        public async Task<ProfileModel> GetProfileAsync(string accessToken)
        {
            var result = await HttpClientHelper.GetAsync<ProfileModel>("https://api.line.me/v2/profile", customHeader: new Dictionary<string, string>() { { "Authorization", $"Bearer {accessToken}" } });
            if (result.StatusCode != HttpStatusCode.OK.ToInt()) throw new Exception("Line Get GetProfileAsync Failed");

            return result.Data;
        }


    }
}
