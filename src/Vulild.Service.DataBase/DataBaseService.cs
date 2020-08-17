using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Vulild.Core;

namespace Vulild.Service.DataBase
{
    public delegate void OnDataBaseServiceDispose(IDbConnection service);
    public abstract class DataBaseService : IDataBaseService/*, IDisposable*/
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
            IDbConnection conn = _ThisOption.GetDbConnection(false);
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
    }
}
