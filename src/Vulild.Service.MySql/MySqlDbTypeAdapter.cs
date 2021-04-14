using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Vulild.Core.Orm;
using Vulild.Service.DataBase;

namespace Vulild.Service.MySql
{
    public class MySqlDbTypeAdapter : IDbTypeAdapter
    {
        public string Convert2DbType(PropertyInfo attr)
        {
            //var attr = pi.GetCustomAttribute<DbFieldAttribute>();
            //string fieldName = pi.Name;
            //string type = "varchar(255)";
            //if (pi.PropertyType == typeof(int))
            //{
            //    type = "int";
            //}
            //if (pi.PropertyType == typeof(long))
            //{
            //    type = "bigint";
            //}
            //if (pi.PropertyType == typeof(string))
            //{
            //    type = "varchar(255)";
            //}
            //if (pi.PropertyType == typeof(DateTime))
            //{
            //    type = "DateTime";
            //}
            //string isNull = "null";
            //if (attr != null)
            //{
            //    //if (!string.IsNullOrWhiteSpace(attr.FieldName))
            //    //{
            //    //    fieldName = attr.FieldName;
            //    //}
            //    //if (!string.IsNullOrWhiteSpace(attr.Type))
            //    //{
            //    //    type = attr.Type;
            //    //}
            //    //isNull = attr.IsNull ? "null" : "not null";
            //}
            //return $"{attr.FieldName} {attr.Type} {attr.IsNull}";
            return "";

        }
    }
}
