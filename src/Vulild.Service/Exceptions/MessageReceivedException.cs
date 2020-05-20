using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.Exceptions
{
    public class MessageReceivedException
    {
        public string ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        public Exception InnerException { get; set; }
    }
}
