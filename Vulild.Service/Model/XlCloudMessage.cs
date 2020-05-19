using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.Model
{
    public class XlCloudMessage
    {

    }

    public class XlCloudMessage<T> : XlCloudMessage
    {
        public string Topic { get; set; }

        public string MessageType { get; set; }

        public T Data { get; set; }
    }
}
