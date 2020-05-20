using System;
using System.Collections.Generic;
using System.Text;

namespace Vulild.Service.DataBase.Exceptions
{
    public class DbConnectionOutOfLimitException : Exception
    {
        public DbConnectionOutOfLimitException(string message) : base(message)
        {

        }
        public DbConnectionOutOfLimitException() : this("数据库连接超出限制")
        {

        }
    }
}
