using System;
using System.Collections.Generic;
using System.Text;
using Vulild.Core.Orm;
using Vulild.Service.DataBase;

namespace Vulild.Service.SQLite
{
    public class SQLiteDbTypeAdapter : IDbTypeAdapter
    {
        public string Convert2DbType(DbFieldAttribute pi)
        {
            throw new NotImplementedException();
        }
    }
}
