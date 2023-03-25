using FlarumApi.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;

namespace FlarumApi.Helpers
{
    internal class NetworkHelper
    {
        /// <summary>
        /// 调用Get方法
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public static async Task<JObject> GetAsync(string link,string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:80.0) Gecko/20100101 Firefox/80.0");
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");
            client.DefaultRequestHeaders.Add("Authorization", "Token " + token);
            var response = await client.GetAsync(new Uri(link)); 
            var result = await response.Content.ReadAsStringAsync();
            return JObject.Parse(result);
        }
        public static async Task<JObject> PostAsync(string link,HttpContent httpContent,string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:80.0) Gecko/20100101 Firefox/80.0");//模拟浏览器
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");
            client.DefaultRequestHeaders.Add("Authorization", "Token " + token);
            var response = await client.PostAsync(link, httpContent);
            var result = await response.Content.ReadAsStringAsync();
            return JObject.Parse(result);
        }
        public static async Task<JObject> PostWithJsonAsync(string link, string json, string token,string referer = null)
        {
            var client = new HttpClient();
            if(referer!=null)
                client.DefaultRequestHeaders.Add("referer", referer);
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:80.0) Gecko/20100101 Firefox/80.0");//模拟浏览器
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");
            client.DefaultRequestHeaders.Add("Authorization", "Token " + token);
            var response = await client.PostAsync(link, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            var result = await response.Content.ReadAsStringAsync();
            return JObject.Parse(result);
        }
        public static async Task<JObject> PatchWithJsonAsync(string link, string json, string token, string referer = null)
        {
            var client = new HttpClient();
            if (referer != null)
                client.DefaultRequestHeaders.Add("referer", referer);
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:80.0) Gecko/20100101 Firefox/80.0");//模拟浏览器
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");
            client.DefaultRequestHeaders.Add("x-http-method-override", "PATCH");
            client.DefaultRequestHeaders.Add("Authorization", "Token " + token);
            var response = await client.PostAsync(link, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            var result = await response.Content.ReadAsStringAsync();
            return JObject.Parse(result);
        }
        public static async Task<JObject> UploadAsync(string link, MultipartFormDataContent content, string token, string referer = null)
        {
            var client = new HttpClient();

            if (referer != null)
                client.DefaultRequestHeaders.Add("referer", referer);
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:80.0) Gecko/20100101 Firefox/80.0");//模拟浏览器
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");
            client.DefaultRequestHeaders.Add("Authorization", "Token " + token);
            var response = await client.PostAsync(new Uri(link), content);//上传
             var result = await response.Content.ReadAsStringAsync();
            return JObject.Parse(result);
        }
    }
}
