using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.Exceptions
{
    public class TypeNotFoundException : Exception
    {
        public TypeNotFoundException(string type) : base($"未配置类型{type}的映射关系")
        {

        }
    }
}
