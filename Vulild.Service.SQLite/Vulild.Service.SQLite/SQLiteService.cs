using System;
using System.Collections.Generic;
using System.Data;
using Vulild.Service.DataBase;

namespace Vulild.Service.SQLite
{
    public class SQLiteService : DataBaseService
    {
        public override IDbDataParameter GetParameter(KeyValuePair<string, object> value)
        {
            throw new NotImplementedException();
        }

        public override string GetParameterName(string param)
        {
            throw new NotImplementedException();
        }
    }
}
