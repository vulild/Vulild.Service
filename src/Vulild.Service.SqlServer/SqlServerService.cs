using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vulild.Service.Attributes;
using Vulild.Service.DataBase;

namespace Vulild.Service.SqlServer
{
    [ServiceOption(Type = typeof(SqlServerServieOption))]
    public class SqlServerService : DataBaseService
    {
        public override void CreateTable<T>()
        {
            throw new NotImplementedException();
        }

        public override IDbDataParameter GetParameter(KeyValuePair<string, object> value)
        {
            return new SqlParameter(value.Key, value.Value);
        }

        public override string GetParameterName(string param)
        {
            return $"@{param}";
        }

        public override bool TableExist(string tableName)
        {
            throw new NotImplementedException();
        }

        protected override string GetPagingSql(string sql, string orders, int pageNum, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(orders))
            {
                orders = "id";
            }
            return $@"select top {pageSize} * 
from(select row_number()
over(order by {orders}) as rownumber, *
from ({sql}) temp) temp_row
where rownumber > (({pageNum} - 1) * {pageSize}); ";
        }
    }
}
