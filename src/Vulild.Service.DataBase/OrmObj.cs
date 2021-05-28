using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Vulild.Core.Algorithms;
using Vulild.Core.Orm;
using Vulild.Core.Orm;

namespace Vulild.Service.DataBase
{
    public abstract class OrmObj : IDbUpdate, IDbInsert, IDbDelete
    {
        [DbField(FieldName = "Id", IsNull = false, Type = "bigint")]
        public long Id { get; set; }

        [NotDbField]
        public virtual string TableName
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public int Delete()
        {
            var db = ServiceUtil.GetService<IDataBaseService>();
            string sql = $"delete from {TableName} where id={db.GetParameterName("Id")}";

            return db.ExecuteNonQuery(sql, new Dictionary<string, object> { { "Id", Id } });
        }

        private Dictionary<string, object> getColumns()
        {
            Dictionary<string, object> pisDic = new Dictionary<string, object>();
            Type type = GetType();
            PropertyInfo[] pis = type.GetProperties();
            foreach (var pi in pis)
            {
                var attr = pi.GetCustomAttribute<NotDbFieldAttribute>();
                if (attr == null)
                {
                    pisDic.Add(pi.Name, pi.GetValue(this));
                }
            }
            return pisDic;
        }

        public string GetInsertSql()
        {
            var colDic = getColumns();
            string columnsSql = string.Join(" , ", colDic.Keys);
            string valueSql = string.Join(" ',' ", colDic.Values);

            string sql = $"insert into {TableName}({columnsSql})values('{valueSql}')";
            return sql;
        }

        public int Insert()
        {
            var db = ServiceUtil.GetService<IDataBaseService>();

            Id = Snowflake.Instance.NewId();

            var colDic = getColumns();
            string columnsSql = string.Join(" , ", colDic.Keys);
            string valueSql = string.Join(",", colDic.Select(a => db.GetParameterName(a.Key)));

            string sql = $"insert into {TableName}({columnsSql})values({valueSql})";
            return db.ExecuteNonQuery(sql, colDic);
        }

        public int Update()
        {
            var db = ServiceUtil.GetService<IDataBaseService>();

            var colDic = getColumns();

            string colSql = string.Join(",", colDic.Select(a => $"{a.Key}={db.GetParameterName(a.Key)}"));

            string sql = $"update {TableName} set {colSql} where id={db.GetParameterName("Id")}";

            return db.ExecuteNonQuery(sql, colDic);
        }

        public int Save()
        {
            if (Id == 0)
            {
                return Insert();
            }
            else
            {
                return Update();
            }
        }
    }
}
