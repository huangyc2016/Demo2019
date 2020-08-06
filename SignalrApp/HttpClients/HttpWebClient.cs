
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SignalrApp.HttpClients
{
    public class HttpWebClient : IHttpWebClient
    {
        private readonly IHttpClientFactory _clientFactory;
        public HttpWebClient(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> GetAsync(string requestUri, ApiParameters parameters, string token)
        {
            //拼接带参数的地址
            string parametersUri = "";
            if (parameters != null)
            {
                List<string> datalist = new List<string>();
                Type type = parameters.GetType();

                PropertyInfo[] props = type.GetProperties();
                foreach (PropertyInfo item in props)
                {
                    string name = item.Name;
                    object value = item.GetValue(parameters);
                    datalist.Add(string.Format("{0}={1}", name, value));
                }
                parametersUri = string.Join("&", datalist);
                requestUri = requestUri + "?";
            }
            requestUri += parametersUri;

            //从工厂获取请求对象
            var client = _clientFactory.CreateClient("github");
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();

            }
            else
            {  
                //记录日志
                return "";
            }

        }

        public async Task<string> PostAsync(string requestUri, ApiParameters parameters, string token)
        {
            var parameterJson = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");

            //从工厂获取请求对象
            var client = _clientFactory.CreateClient("github");
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }


            using var response =
        await client.PostAsync(requestUri, parameterJson);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();

            }
            else
            {
                //记录日志
                return "";
            }

        }
    }
}
