using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Vulild.Core;
using Vulild.Core.Orm;

namespace Vulild.Service.DataBase
{
    public interface IDataBaseService : IRemoteService
    {
        /// <summary>
        /// 增删改
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dbParams">参数化</param>
        /// <returns></returns>
        int ExecuteNonQuery(string sql, Dictionary<string, object> dbParams);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dbParams">参数化</param>
        /// <param name="readAction">读取IDataReader</param>
        void ExecuteQuery(string sql, Dictionary<string, object> dbParams, Action<IDataReader> readAction);

        List<T> ExecuteQuery<T>(string sql, Dictionary<string, object> dbParams) where T : new();

        List<T> ExecuteQuery<T>(Dictionary<string, object> where) where T : new();

        /// <summary>
        /// 读取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dbParams">参数化</param>
        /// <returns></returns>
        object ExecuteScalar(string sql, Dictionary<string, object> dbParams);

        ///// <summary>
        ///// 变更连接数据库
        ///// </summary>
        ///// <param name="database"></param>
        //void ChangeDataBase(string database);

        /// <summary>
        /// 转换参数占位符
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        string GetParameterName(string param);

        /// <summary>
        /// 转换参数化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IDbDataParameter GetParameter(KeyValuePair<string, object> value);

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="executes"></param>
        /// <returns></returns>
        bool ExecuteTransaction(params Func<IDbCommand, bool>[] executes);

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="executes"></param>
        /// <returns></returns>
        bool ExecuteTransaction(IEnumerable<Func<IDbCommand, bool>> executes);

        bool TableExist(string tableName);

        bool TableExist<T>();

        void CreateTable<T>();

        //string GetPageSql(string sql, int pageNum, int pageSize);

        //List<T> GetPageData<T>(string sql, int pageNum, int pageSize) where T : new();
    }
}
