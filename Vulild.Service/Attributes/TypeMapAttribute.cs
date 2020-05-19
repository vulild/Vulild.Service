using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.Attributes
{
    public class TypeMapAttribute : Attribute
    {
        public string RemoteKey { get; set; }
    }
}
