using System;
using System.Collections.Generic;
using System.Text;
using Vulild.Service;

namespace Vulild.HttpService
{
    public interface IHttpPostJsonService : IService
    {
        OUT PostJson<IN, OUT>(string url, IN param);
    }
}
