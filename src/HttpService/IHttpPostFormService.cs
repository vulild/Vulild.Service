using System;
using System.Collections.Generic;
using System.Text;
using Vulild.Service;

namespace Vulild.HttpService
{
    public interface IHttpPostFormService : IService
    {
        OUT PostForm<OUT>(string url, System.Collections.Generic.Dictionary<string, string> param);
    }
}
