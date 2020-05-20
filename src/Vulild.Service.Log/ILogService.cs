using System;
using System.Collections.Generic;
using System.Text;

namespace Vulild.Service.Log
{
    public interface ILogService : IService
    {
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="log"></param>
        void WriteLog(string log);
    }
}
