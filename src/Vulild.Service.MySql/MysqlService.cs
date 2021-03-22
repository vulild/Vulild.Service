using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Vulild.Core.FormatConversion;
using Vulild.Core.Orm;
using Vulild.Service.Attributes;
using Vulild.Service.DataBase;

namespace Vulild.Service.MySql
{
    [ServiceOption(Type = typeof(MySqlServiceOption))]
    public class MysqlService : DataBaseService
    {
        public override void CreateTable<T>()
        {
            IDbTypeAdapter adapter = new DefaultDbTypeAdapter();
            Type type = typeof(T);
            var pis = type.GetProperties();

            List<string> fields = new List<string>();
            foreach (var pi in pis)
            {
                var attr = pi.GetCustomAttribute<DbFieldAttribute>();
                if (attr != null)
                {
                    fields.Add(adapter.Convert2DbType(attr));
                }
            }
            if (fields.Any())
            {
                string strField = String.Join(",", fields);
                string createSql = $"create table {type.Name} ({strField})";
                ExecuteNonQuery(createSql, null);
            }
        }

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

        public override bool TableExist(string tableName)
        {
            var conn = _ThisOption.GetDbConnection(true);
            try
            {
                string sql = $"SELECT Count(*) FROM information_schema.TABLE_CONSTRAINTS WHERE table_name ='{tableName}' and Table_Schema='{conn.Database}'";
                int? tableCount = ExecuteScalar(sql, null).ToIntNull();

                if (tableCount == null || tableCount == 0)
                {
                    return false;
                }
                return true;
            }
            finally
            {
                FreeDbConnection(conn);
            }
        }

        protected override string GetPagingSql(string sql, int pageNum, int pageSize)
        {
            int startIndex = (pageNum - 1) * pageSize;
            return $"{sql} limit  {startIndex},{pageSize}";
        }
    }
}
