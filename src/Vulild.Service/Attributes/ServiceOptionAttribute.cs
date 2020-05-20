using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.Attributes
{
    /// <summary>
    /// 指定服务对应的配置类
    /// </summary>
    public class ServiceOptionAttribute : Attribute
    {
        public Type Type;
    }
}
