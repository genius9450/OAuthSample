using OAuth.Sample.Domain.Enum;
using OAuth.Sample.Domain.Shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OAuth.Sample.Api.Helper
{
    public static class HttpClientHelper
    {
        /// <summary>
        /// 呼叫API
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static async Task<ResponseModel<TResponse>> PostAsync<TResponse>(string url, object data, string mediaType = "application/x-www-form-urlencoded")
        {
            var result = new ResponseModel<TResponse>();
            var postData = string.Empty;
            try
            {
                HttpClient client = new HttpClient();

                if (mediaType == "application/x-www-form-urlencoded")
                    postData = GetQueryString(data);
                else
                    postData = JsonConvert.SerializeObject(data);

                StringContent content = new StringContent(postData, Encoding.UTF8, mediaType);                

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content                    
                };

                HttpResponseMessage response = await client.SendAsync(requestMessage);

                if (response.StatusCode.ToString() == HttpStatusCode.OK.ToString())
                {
                    result = new ResponseModel<TResponse>()
                    {
                        StatusCode = HttpStatusCode.OK.ToInt(),
                        Msg = HttpStatusCode.OK.Description(),
                        Data = JsonConvert.DeserializeObject<TResponse>(response.Content.ReadAsStringAsync().Result)
                    };
                }
                else
                {
                    result = new ResponseModel<TResponse>()
                    {
                        StatusCode = response.StatusCode.ToInt(),
                        Msg = response.StatusCode.Description()
                    };
                }                

            }
            catch (Exception ex)
            {
                Const.Logger.LogError(ex, "{HttpMethod} / {FullPath} / {MediaType} / {Request} / {Response}", "POST", url, mediaType, postData, JsonConvert.SerializeObject(result));
                throw ex;
            }
            finally
            {
                Const.Logger.LogInformation("{HttpMethod} / {FullPath} / {MediaType} / {Request} / {Response}", "POST", url, mediaType, postData, JsonConvert.SerializeObject(result));
            }

            return result;
        }

        /// <summary>
        /// 呼叫API
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<ResponseModel<TResponse>> GetAsync<TResponse>(string url, object data = null)
        {            
            var Result = new ResponseModel<TResponse>();
            var parameter = string.Empty;
            try
            {
                HttpClient client = new HttpClient();
                
                if (data != null)
                {
                    parameter = GetQueryString(data);
                    url = $"{url}?{parameter}";
                }

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.StatusCode.ToString() == HttpStatusCode.OK.ToString())
                {
                    Result = new ResponseModel<TResponse>()
                    {
                        StatusCode = HttpStatusCode.OK.ToInt(),
                        Msg = HttpStatusCode.OK.Description(),
                        Data = JsonConvert.DeserializeObject<TResponse>(response.Content.ReadAsStringAsync().Result)
                    };
                }
                else
                {
                    Result = new ResponseModel<TResponse>()
                    {
                        StatusCode = response.StatusCode.ToInt(),
                        Msg = response.StatusCode.Description()
                    };
                }

            }
            catch (Exception ex)
            {
                Const.Logger.LogError(ex, "{HttpMethod} / {FullPath} / {Request} / {Response}", "Get", url, parameter, JsonConvert.SerializeObject(Result));
                throw ex;
            }
            finally
            {
                Const.Logger.LogInformation("{HttpMethod} / {FullPath} / {Request} / {Response}", "Get", url, parameter, JsonConvert.SerializeObject(Result));
            }

            return Result;
        }

        /// <summary>
        /// 取得Object query string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }

    }
}

