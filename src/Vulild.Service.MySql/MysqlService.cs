using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using Vulild.Service.Attributes;
using Vulild.Service.DataBase;

namespace Vulild.Service.MySql
{
    [ServiceOption(Type = typeof(MySqlServiceOption))]
    public class MysqlService : DataBaseService
    {
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
