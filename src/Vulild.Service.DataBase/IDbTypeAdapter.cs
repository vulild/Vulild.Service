using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Vulild.Core.Orm;

namespace Vulild.Service.DataBase
{
    public interface IDbTypeAdapter
    {
        string Convert2DbType(DbFieldAttribute pi);
    }
}
