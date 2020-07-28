using System;
using System.Collections.Generic;
using System.Text;

namespace Vulild.Service.Services
{
    interface IServiceInitService : IService
    {
        IEnumerable<Option> GetOptions();
    }
}
