using System;
using System.Collections.Generic;
using System.Text;

namespace Vulild.Service.DataBase.Exceptions
{
    public class DbConnectionNullException : Exception
    {
        public DbConnectionNullException(string message) : base(message)
        {

        }
    }
}
