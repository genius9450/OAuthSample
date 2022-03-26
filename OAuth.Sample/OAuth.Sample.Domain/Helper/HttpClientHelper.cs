using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OAuth.Sample.Domain.Shared;

namespace OAuth.Sample.Domain.Helper
{
    public static class HttpClientHelper
    {
        public static ILogger Logger;

        /// <summary>
        /// 呼叫API
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static async Task<ResponseModel<TResponse>> PostAsync<TResponse>(string url, object data, string mediaType = "application/x-www-form-urlencoded", Dictionary<string, string> customHeader = null)
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

                // 客製化Header
                if (customHeader != null && customHeader.Any())
                {
                    foreach (var h in customHeader)
                    {
                        requestMessage.Headers.Add(h.Key, h.Value);
                    }
                }

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
                Logger.LogError(ex, "{HttpMethod} / {FullPath} / {MediaType} / {Request} / {Response}", "POST", url, mediaType, postData, JsonConvert.SerializeObject(result));
                throw ex;
            }
            finally
            {
                Logger.LogInformation("{HttpMethod} / {FullPath} / {MediaType} / {Request} / {Response}", "POST", url, mediaType, postData, JsonConvert.SerializeObject(result));
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
        public static async Task<ResponseModel<TResponse>> GetAsync<TResponse>(string url, object data = null, Dictionary<string, string> customHeader = null)
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

                StringContent content = new StringContent(parameter, Encoding.UTF8);

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url)
                {
                    Content = content
                };

                // 客製化Header
                if (customHeader != null && customHeader.Any())
                {
                    foreach (var h in customHeader)
                    {
                        requestMessage.Headers.Add(h.Key, h.Value);
                    }
                }

                HttpResponseMessage response = await client.SendAsync(requestMessage); //client.GetAsync(url);

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
                Logger.LogError(ex, "{HttpMethod} / {FullPath} / {Request} / {Response}", "Get", url, parameter, JsonConvert.SerializeObject(Result));
                throw ex;
            }
            finally
            {
                Logger.LogInformation("{HttpMethod} / {FullPath} / {Request} / {Response}", "Get", url, parameter, JsonConvert.SerializeObject(Result));
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
            if (obj == null) return string.Empty;

            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }

    }
}

