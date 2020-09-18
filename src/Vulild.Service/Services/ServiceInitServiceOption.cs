using System;
using System.Collections.Generic;
using System.Text;
using Vulild.Service.Attributes;

namespace Vulild.Service.Services
{
    [ServiceOption(Type = typeof(ServiceInitFromJsonFile))]
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
