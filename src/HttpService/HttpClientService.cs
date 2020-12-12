using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.IO;

namespace HttpService
{
    public class HttpClientService : IHttpService
    {
        private HttpClient _HttpClient;

        public HttpSend BeforeSend { get; set; }
        public HttpSend AfterSend { get; set; }

        public HttpClient GetHttpClient()
        {
            if (_HttpClient == null)
            {
                _HttpClient = new HttpClient();
                _HttpClient.DefaultRequestHeaders.ExpectContinue = false;
            }
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

        public OUT Post<IN, OUT>(string url, IN param)
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
    }
}
