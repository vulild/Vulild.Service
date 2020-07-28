using System;
using System.Collections.Generic;
using System.Text;

namespace Vulild.Service.Services
{
    public class ServiceInitServiceOption : Option
    {
        public string FileName { get; set; }
        public override IService CreateService()
        {
            return new ServiceInitFromJsonFile
            {
                FileName = this.FileName
            };
        }
    }
}
