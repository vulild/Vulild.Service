using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Vulild.Core.FormatConversion;
using Vulild.Core.Orm;

namespace Vulild.Service.DataBase
{
    public delegate void OnDataBaseServiceDispose(IDbConnection service);
    public abstract class DataBaseService : IDataBaseService, IPagingService/*, IDisposable*/
    {
        /// <summary>
        /// 连接使用完成释放时触发的事件
        /// </summary>
        public event OnDataBaseServiceDispose OnConnectionFree;
        public Option Option { get; set; }

        public DataBaseServiceOption _ThisOption
        {
            get
            {
                return (DataBaseServiceOption)Option;
            }
        }

        /// <summary>
        /// 释放连接
        /// </summary>
        /// <param name="conn"></param>
        protected void FreeDbConnection(IDbConnection conn)
        {
            FreeDbConnection(conn, false);
        }

        /// <summary>
        /// 释放连接
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="forceClose">是否强制释放，强制释放表示不放回连接池，直接关闭连接</param>
        protected void FreeDbConnection(IDbConnection conn, bool forceClose)
        {
            if (forceClose || OnConnectionFree == null)
            {
                conn.Close();
            }
            else
            {
                OnConnectionFree(conn);
            }
        }

        public int ExecuteNonQuery(string sql, Dictionary<string, object> dbParams)
        {
            IDbConnection conn = _ThisOption.GetDbConnection(true);
            try
            {
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (dbParams != null)
                    {
                        foreach (var param in dbParams)
                        {
                            cmd.Parameters.Add(GetParameter(param));
                        }
                    }

                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
            }
            finally
            {
                FreeDbConnection(conn);
            }
        }

        public void ExecuteQuery(string sql, Dictionary<string, object> dbParams, Action<IDataReader> readAction)
        {
            IDbConnection conn = _ThisOption.GetDbConnection();
            try
            {
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (dbParams != null)
                    {
                        foreach (var param in dbParams)
                        {
                            cmd.Parameters.Add(GetParameter(param));
                        }
                    }

                    var dr = cmd.ExecuteReader();
                    try
                    {
                        readAction(dr);
                    }
                    finally
                    {
                        dr.Close();
                    }

                }
            }
            finally
            {
                FreeDbConnection(conn);
            }
        }

        public List<T> ExecuteQuery<T>(string sql, Dictionary<string, object> dbParams) where T : new()
        {
            List<T> lst = new List<T>();
            ExecuteQuery(sql, dbParams, dr =>
            {
                lst = dr.ToList<T>();
            });
            return lst;
        }

        public object ExecuteScalar(string sql, Dictionary<string, object> dbParams)
        {
            IDbConnection conn = _ThisOption.GetDbConnection();
            try
            {
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (dbParams != null)
                    {
                        foreach (var param in dbParams)
                        {
                            cmd.Parameters.Add(GetParameter(param));
                        }
                    }

                    var obj = cmd.ExecuteScalar();
                    return obj;
                }
            }
            finally
            {
                FreeDbConnection(conn);
            }
        }

        public bool ExecuteTransaction(params Func<IDbCommand, bool>[] executes)
        {
            IDbConnection conn = _ThisOption.GetDbConnection();
            try
            {
                using (var trans = conn.BeginTransaction())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = trans;
                        try
                        {
                            foreach (var execute in executes)
                            {
                                if (!execute(cmd))
                                {
                                    trans.Rollback();
                                    return false;
                                }
                            }
                            trans.Commit();
                            return true;
                        }
                        catch (Exception)
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            finally
            {
                FreeDbConnection(conn, true);
            }

        }

        public abstract IDbDataParameter GetParameter(KeyValuePair<string, object> value);

        public abstract string GetParameterName(string param);

        public virtual int GetCount(string sql, Dictionary<string, object> dbParams)
        {
            string countSql = GetCountSql(sql);//$"select count(*) from ({sql}) a";

            return ExecuteScalar(countSql, dbParams).ToInt();
        }

        public List<T> ExecuteQuery<T>(Dictionary<string, object> wheres) where T : new()
        {
            string sql = $"select * from {typeof(T).Name} ";
            if (wheres != null && wheres.Any())
            {
                string whereParam = "";
                foreach (var where in wheres)
                {
                    if (!string.IsNullOrWhiteSpace(whereParam))
                    {
                        whereParam = $"{whereParam} and ";
                    }
                    whereParam = $"{whereParam} {where.Key}={GetParameterName(where.Key)}";
                }
                sql = $"{sql} where {whereParam}";
            }
            return ExecuteQuery<T>(sql, wheres);
        }

        public void ExecuteQuery(string sql, string orders, Dictionary<string, object> dbParams, int pageNum, int pageSize, out int pageCount, Action<IDataReader> readAction)
        {
            string pageSql = GetPagingSql(sql, orders, pageNum, pageSize);

            pageCount = GetCount(sql, dbParams);

            ExecuteQuery(pageSql, dbParams, readAction);
        }

        public List<T> ExecuteQuery<T>(string sql, string orders, Dictionary<string, object> dbParams, int pageNum, int pageSize, out int pageCount) where T : new()
        {
            string pageSql = GetPagingSql(sql, orders, pageNum, pageSize);

            pageCount = GetCount(sql, dbParams);

            return ExecuteQuery<T>(pageSql, dbParams);
        }

        public List<T> ExecuteQuery<T>(Dictionary<string, object> wheres, string orders, int pageNum, int pageSize, out int pageCount) where T : new()
        {
            string sql = $"select * from {typeof(T).Name} ";
            if (wheres != null && wheres.Any())
            {
                string whereParam = "";
                foreach (var where in wheres)
                {
                    if (!string.IsNullOrWhiteSpace(whereParam))
                    {
                        whereParam = $"{whereParam} and ";
                    }
                    whereParam = $"{whereParam} {where.Key}={GetParameterName(where.Key)}";
                }
                sql = $"{sql} where {whereParam}";
            }

            string pageSql = GetPagingSql(sql, orders, pageNum, pageSize);

            pageCount = GetCount(sql, wheres);

            return ExecuteQuery<T>(pageSql, wheres);
        }

        protected abstract string GetPagingSql(string sql, string orders, int pageNum, int pageSize);

        protected virtual string GetCountSql(string sql)
        {
            return $"select count(*) from ({sql}) a";
        }

        public abstract bool TableExist(string tableName);

        public bool TableExist<T>()
        {
            return TableExist(typeof(T).Name);
        }

        public abstract void CreateTable<T>();

        //public abstract string GetPageSql(string sql, int pageNum, int pageSize);

        //public List<T> GetPageData<T>(string sql, int pageNum, int pageSize, Dictionary<string, object> dbParams) where T : new()
        //{
        //    string pageSql = GetPageSql(sql, pageNum, pageSize);

        //    return ExecuteQuery<T>(pageSql, dbParams);
        //}
    }
}
