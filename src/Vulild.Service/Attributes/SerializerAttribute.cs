using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.Attributes
{
    public class SerializerAttribute : Attribute
    {
        public Type SerializerType { get; set; }
    }
}
