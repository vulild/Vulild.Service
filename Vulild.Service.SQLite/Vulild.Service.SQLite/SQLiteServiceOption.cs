using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using Vulild.Service.DataBase;

namespace Vulild.Service.SQLite
{
    public class SQLiteServiceOption : DataBase.DataBaseServiceOption
    {
        public string FileName { get; set; }
        protected override DataBaseService GetService()
        {
            return new SQLiteService();
        }

        protected override IDbConnection GetRealDb()
        {
            return new SQLiteConnection(FileName);
        }
    }
}
