using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Vulild.Core.Orm;
using Vulild.Core.Orm;

namespace Vulild.Service.DataBase
{
    public abstract class OrmObj : IDbUpdate, IDbInsert, IDbDelete
    {
        public abstract string Id { get; }

        protected virtual string TableName
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
            FieldInfo[] fis = type.GetFields();
            foreach (var fi in fis)
            {
                pisDic.Add(fi.Name, fi.GetValue(this));
            }
            return pisDic;
        }

        public int Insert()
        {
            var db = ServiceUtil.GetService<IDataBaseService>();

            var colDic = getColumns();
            colDic.Add("Id", Guid.NewGuid().ToString());
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

            colDic.Add("Id", Id);
            return db.ExecuteNonQuery(sql, colDic);
        }
    }
}
