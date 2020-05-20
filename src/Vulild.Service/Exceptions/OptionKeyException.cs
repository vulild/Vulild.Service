using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.Exceptions
{
    /// <summary>
    /// 配置的主键未找到或者重复抛出的异常
    /// </summary>
    public class OptionKeyException : Exception
    {
        public OptionKeyException(string message) : base(message)
        {

        }

        public OptionKeyException() : base("配置键不能为空且不能重复")
        {

        }
    }
}
