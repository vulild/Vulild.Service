using Vulild.Service.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.Model
{
    public class XlCloudMessageData
    {
    }


    [TypeMap(RemoteKey = "123")]
    public class TestData : XlCloudMessageData
    {
        public long Ticks;
    }

}
