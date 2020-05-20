using Vulild.Service.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service
{
    /// <summary>
    /// 中间件服务接口，如mq，redis，数据库等
    /// </summary>
    public interface IRemoteService : IService
    {

    }
}
