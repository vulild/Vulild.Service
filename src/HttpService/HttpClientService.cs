using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.IO;
using Vulild.Service;

namespace Vulild.HttpService
{
    [Service.Attributes.ServiceOption(Type = typeof(HttpClientServiceOption))]
    internal class HttpClientService : IHttpService
    {
        internal HttpClient _HttpClient;

        public HttpSend BeforeSend { get; set; }
        public HttpSend AfterSend { get; set; }
        public Option Option { get; set; }

        public HttpClient GetHttpClient()
        {
            //if (_HttpClient == null)
            //{
            //    _HttpClient = new HttpClient();
            //    _HttpClient.DefaultRequestHeaders.ExpectContinue = false;
            //}
            return _HttpClient;

        }
        public OUT Get<OUT>(string url)
        {
            var hc = GetHttpClient();
            var res = hc.GetAsync($"{url}").Result;
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                OUT resModel = JsonConvert.DeserializeObject<OUT>(result);
                return resModel;
            }
            throw new HttpException(res.StatusCode);
        }

        public void GetFile(string url, string path)
        {
            var hc = GetHttpClient();
            var byts = hc.GetByteArrayAsync(url).Result;
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.Write(byts, 0, byts.Length);
                fs.Close();
            }
        }

        public OUT PostJson<IN, OUT>(string url, IN param)
        {
            var hc = GetHttpClient();
            BeforeSend?.Invoke(hc);
            JsonSerializerSettings settings = new JsonSerializerSettings();

            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            StringContent sc = new StringContent(JsonConvert.SerializeObject(param, settings));
            sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
            {
                CharSet = "UTF-8"
            };

            var res = hc.PostAsync($"{url}", sc).Result;
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                OUT resModel = JsonConvert.DeserializeObject<OUT>(result);

                return resModel;

            }
            throw new HttpException(res.StatusCode);
        }

        public OUT PostForm<OUT>(string url, Dictionary<string, string> param)
        {
            var hc = GetHttpClient();
            BeforeSend?.Invoke(hc);

            FormUrlEncodedContent content = new FormUrlEncodedContent(param);

            var res = hc.PostAsync($"{url}", content).Result;
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                OUT resModel = JsonConvert.DeserializeObject<OUT>(result);

                return resModel;

            }
            throw new HttpException(res.StatusCode);
        }
    }
}
