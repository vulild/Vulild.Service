using System;
using System.Net.Http;

namespace HttpService
{
    public delegate void HttpSend(HttpClient httpClient);
    public interface IHttpService
    {
        HttpSend BeforeSend { get; set; }

        HttpSend AfterSend { get; set; }


        OUT Post<IN, OUT>(string url, IN param);

        OUT Get<OUT>(string url);

        void GetFile(string url, string path);
    }
}
