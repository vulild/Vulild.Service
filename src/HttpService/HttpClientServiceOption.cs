using System;
using System.Collections.Generic;
using System.Text;
using Vulild.Service;

namespace Vulild.HttpService
{
    public class HttpClientServiceOption : Service.Option
    {
        public override IService CreateService()
        {
            HttpClientService httpClientService = new HttpClientService();
            httpClientService._HttpClient = new System.Net.Http.HttpClient();
            return httpClientService;
        }
    }
}
