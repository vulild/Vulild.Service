﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Vulild.Service.Services
{
    public interface IServiceInitService : IService
    {
        IEnumerable<Option> GetOptions();
    }
}
