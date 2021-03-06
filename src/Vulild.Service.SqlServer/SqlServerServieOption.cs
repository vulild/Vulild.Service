﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vulild.Service.DataBase;

namespace Vulild.Service.SqlServer
{
    public class SqlServerServieOption : DataBaseServiceOption
    {
        protected override DataBaseService GetService()
        {
            SqlServerService service = new SqlServerService();
            
            return service;
        }

        protected override IDbConnection GetRealDb()
        {
            var conn = new SqlConnection($"Data Source={Host},{Port};Initial Catalog={DataBase}; User Id={UserName};Password={Password}");
            conn.Open();

            return conn;

        }
    }
}
