using System;
using System.Collections.Generic;
using System.Text;
using Vulild.Core.Orm;

namespace Vulild.Service.DataBase
{
    public class DefaultDbTypeAdapter : IDbTypeAdapter
    {
        public string Convert2DbType(DbFieldAttribute attr)
        {
            return $"{attr.FieldName} {attr.Type} {(attr.IsNull?"null":"not null")}";
        }
    }
}
