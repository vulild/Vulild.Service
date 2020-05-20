using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.Exceptions
{
    /// <summary>
    /// 未找到相关配置抛出的异常
    /// </summary>
    public class OptionNotFoundException : Exception
    {
        public OptionNotFoundException(string message) : base(message)
        {

        }
        public OptionNotFoundException() : base("未找到相关配置信息")
        {

        }
    }
}
