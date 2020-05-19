using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.Exceptions
{
    public class ServiceOptionRepeatException : Exception
    {
        public ServiceOptionRepeatException() : base("服务与配置的映射关系重复")
        {

        }

        public ServiceOptionRepeatException(string message) : base(message)
        {

        }
    }
}
