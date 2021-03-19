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
        public override IDbDataParameter GetParameter(KeyValuePair<string, object> value)
        {
            return new SqlParameter(value.Key, value.Value);
        }

        public override string GetParameterName(string param)
        {
            return $"@{param}";
        }

        protected override string GetPagingSql(string sql, int pageNum, int pageSize)
        {
            return $@"select top {pageSize} * 
from(select row_number()
over(order by id asc) as rownumber, *
from ({sql})) temp_row
where rownumber > (({pageNum} - 1) * {pageSize}); ";
        }
    }
}
