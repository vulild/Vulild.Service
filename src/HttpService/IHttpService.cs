using System;
using System.Net.Http;

namespace Vulild.HttpService
{
    public delegate void HttpSend(HttpClient httpClient);
    public interface IHttpService
    {
        HttpSend BeforeSend { get; set; }

        HttpSend AfterSend { get; set; }


        OUT PostJson<IN, OUT>(string url, IN param);

        OUT PostForm<OUT>(string url, System.Collections.Generic.Dictionary<string, string> param);

        OUT Get<OUT>(string url);

        void GetFile(string url, string path);
    }
}
