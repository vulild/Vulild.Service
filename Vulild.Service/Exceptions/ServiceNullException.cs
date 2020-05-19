using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.Exceptions
{
    /// <summary>
    /// 未找到服务抛出的异常
    /// </summary>
    public class ServiceNullException : Exception
    {
        public ServiceNullException() : base("未找到服务")
        {

        }

        public ServiceNullException(string message) : base(message)
        {

        }
    }
}
