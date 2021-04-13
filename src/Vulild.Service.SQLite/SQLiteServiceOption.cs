using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Vulild.Service.Attributes;
using Vulild.Service.DataBase;

namespace Vulild.Service.SQLite
{
    [ServiceOption(Type = typeof(SQLiteService))]
    public class SQLiteServiceOption : DataBase.DataBaseServiceOption
    {
        public string FileName { get; set; }

        public string Password { get; set; }

        protected override DataBaseService GetService()
        {
            return new SQLiteService();
        }

        protected override IDbConnection GetRealDb()
        {
            var conn = new SqliteConnection($"data source={FileName}{(!string.IsNullOrWhiteSpace(Password) ? $";Password={Password}" : "")}");
            conn.Open();
            return conn;
        }
    }
}
