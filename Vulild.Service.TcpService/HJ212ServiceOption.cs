using System;
using System.Collections.Generic;
using System.Text;

namespace Vulild.Service.TcpService
{
    public class HJ212ServiceOption : TcpServerServiceOption
    {
        public override IService CreateService()
        {
            throw new NotImplementedException();
        }

        public override void DoReceiveData(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
