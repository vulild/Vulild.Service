using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.Exceptions
{
    public class TypeRepeatException : Exception
    {
        public TypeRepeatException() : base("类型映射重复")
        {

        }
        public TypeRepeatException(string message) : base(message)
        {

        }
    }
}
