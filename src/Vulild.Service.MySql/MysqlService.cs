using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using Vulild.Core.FormatConversion;
using Vulild.Service.Attributes;
using Vulild.Service.DataBase;

namespace Vulild.Service.MySql
{
    [ServiceOption(Type = typeof(MySqlServiceOption))]
    public class MysqlService : DataBaseService
    {
        //public override string GetPageSql(string sql, int pageNum, int pageSize)
        //{
        //    int startIndex = (pageNum - 1) * pageSize;
        //    return $"{sql} limit  {startIndex},{pageSize}";
        //}

        public override IDbDataParameter GetParameter(KeyValuePair<string, object> value)
        {
            return new MySqlParameter(value.Key, value.Value);
        }

        public override string GetParameterName(string param)
        {
            return $"?{param}";
        }
    }
}
