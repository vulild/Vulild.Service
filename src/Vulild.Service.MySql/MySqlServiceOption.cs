using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using Vulild.Service.DataBase;
using Vulild.Service.Log;

namespace Vulild.Service.MySql
{
    public class MySqlServiceOption : DataBaseServiceOption
    {
        public int? CommandTimeout { get; set; }

        public int? ConnectionLifetime { get; set; }
        public override IService CreateService()
        {
            MysqlService service = new MysqlService();
            service.OnConnectionFree += conn =>
            {
                FreeDbConnection(conn);
            };
            return service;
        }

        protected override IDbConnection GetRealDb()
        {
            var conn = new MySqlConnection("Server=" + this.Host + ";" + (this.Port != 3306 ? "Port=" + this.Port + ";" : "") +
                "Database=" + this.DataBase + ";Uid=" + this.UserName + ";pwd=" + this.Password + ";Connect Timeout=10;" +
                (this.CommandTimeout.HasValue ? "Default Command Timeout=" + this.CommandTimeout.Value + ";" : "") + (this.ConnectionLifetime.HasValue ? "Connection Lifetime=" + this.ConnectionLifetime.Value + ";" : ""));
            conn.Open();

            return conn;

        }
    }
}
