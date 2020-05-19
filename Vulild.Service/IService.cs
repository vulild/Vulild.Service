using Vulild.Service.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service
{
    /// <summary>
    /// 服务接口
    /// </summary>
    public interface IService
    {
        Option Option { get; set; }
    }
}
