using System;
using System.Collections.Generic;
using System.Text;
using Vulild.Service;

namespace Vulild.HttpService
{
    public interface IHttpGetService:IService
    {
        OUT Get<OUT>(string url);
    }
}
