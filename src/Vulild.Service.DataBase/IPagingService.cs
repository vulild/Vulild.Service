using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Vulild.Service.DataBase
{
    public interface IPagingService : IService
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dbParams">参数化</param>
        /// <param name="readAction">读取IDataReader</param>
        void ExecuteQuery(string sql, Dictionary<string, object> dbParams, int pageNum, int pageSize, out int pageCount, Action<IDataReader> readAction);

        List<T> ExecuteQuery<T>(string sql, Dictionary<string, object> dbParams, int pageNum, int pageSize, out int pageCount) where T : new();

        List<T> ExecuteQuery<T>(Dictionary<string, object> where, int pageNum, int pageSize, out int pageCount) where T : new();
    }
}
