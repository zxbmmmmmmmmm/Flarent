using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
        public static async Task<JObject> PostWithJsonAsync(string link, string json, string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:80.0) Gecko/20100101 Firefox/80.0");//模拟浏览器
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");
            client.DefaultRequestHeaders.Add("Authorization", "Token " + token);
            var response = await client.PostAsync(link, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            var result = await response.Content.ReadAsStringAsync();
            return JObject.Parse(result);
        }
    }
}
