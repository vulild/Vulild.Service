using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Vulild.Service.Attributes;
using Vulild.Service.DataBase;

namespace Vulild.Service.SQLite
{
    [ServiceOption(Type = typeof(SQLiteServiceOption))]
    public class SQLiteService : DataBaseService
    {
        public override IDbDataParameter GetParameter(KeyValuePair<string, object> value)
        {
            return new SQLiteParameter(value.Key, value.Value);
        }

        public override string GetParameterName(string param)
        {
            return $"@{param}";
        }

        protected override string GetPagingSql(string sql, int pageNum, int pageSize)
        {
            int startIndex = (pageNum - 1) * pageSize;
            return $"{sql} limit  {startIndex},{pageSize}";
        }
    }
}
