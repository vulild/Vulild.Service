using System;
using System.Collections.Generic;
using System.Text;

namespace Vulild.Service.Log
{
    public interface ILogService : IService
    {
        void WriteLog(string log);
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="log"></param>
        void WriteLog(string log, int level);

        void WriteLog(Exception ex, int level = 4);
    }
}
