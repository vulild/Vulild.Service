using System;
using System.Collections.Generic;
using System.Text;

namespace Vulild.Service.Exceptions
{
    public class ConfigException : Exception
    {
        public ConfigException(string message) : base(message)
        {

        }

        public ConfigException() : base("配置文件异常")
        {

        }
    }
}
