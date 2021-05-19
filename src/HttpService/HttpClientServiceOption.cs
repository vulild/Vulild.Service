using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Vulild.Service;

namespace Vulild.HttpService
{
    public class HttpClientServiceOption : Service.Option
    {
        HttpClient client = new HttpClient();
        public override IService CreateService()
        {
            if (client == null)
            {
                client = new HttpClient();
            }
            HttpClientService httpClientService = new HttpClientService(client);
            return httpClientService;
        }
    }
}
