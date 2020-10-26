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

        public bool Pooling { get; set; } = false;

        public int? MaxPoolSize { get; set; } = 1024;

        public int? MinPoolSize { get; set; } = 10;

        protected override DataBaseService GetService()
        {
            MysqlService service = new MysqlService();

            return service;
        }

        protected override IDbConnection GetRealDb()
        {
            var conn = new MySqlConnection("Server=" + this.Host + ";" + (this.Port != 3306 ? "Port=" + this.Port + ";" : "") +
                "Database=" + this.DataBase + ";Uid=" + this.UserName + ";pwd=" + this.Password + ";Connect Timeout=20;pooling=" + this.Pooling + ";" +
                (this.Pooling ? (this.MaxPoolSize.HasValue ? $"min pool size={MinPoolSize}" : "") + (this.MinPoolSize.HasValue ? ";max pool size={MaxPoolSize};" : "") : "") +
                (this.CommandTimeout.HasValue ? "Default Command Timeout=" + this.CommandTimeout.Value + ";" : "") + (this.ConnectionLifetime.HasValue ? "Connection Lifetime=" + this.ConnectionLifetime.Value + ";" : ""));
            conn.Open();

            return conn;

        }
    }
}
