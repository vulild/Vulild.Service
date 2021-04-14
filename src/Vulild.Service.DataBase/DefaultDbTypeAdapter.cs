using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Vulild.Core.Orm;

namespace Vulild.Service.DataBase
{
    public class DefaultDbTypeAdapter : IDbTypeAdapter
    {
        public virtual string Convert2DbType(PropertyInfo pi)
        {
            string fieldName = pi.Name;
            string type = "";
            int? i = 0;
            bool isNull = false;
            if (pi.PropertyType == typeof(int))
            {
                type = "int";
            }
            else if (pi.PropertyType == typeof(long))
            {
                type = "bigint";
            }
            else if (pi.PropertyType == typeof(decimal) || pi.PropertyType == typeof(double) || pi.PropertyType == typeof(float))
            {
                type = "decimal(16,4)";
            }

            if (pi.PropertyType == typeof(int?))
            {
                type = "int";
                isNull = true;
            }
            else if (pi.PropertyType == typeof(long?))
            {
                type = "bigint";
                isNull = true;
            }
            else if (pi.PropertyType == typeof(decimal?) || pi.PropertyType == typeof(double?) || pi.PropertyType == typeof(float?))
            {
                type = "decimal(16,4)";
                isNull = true;
            }

            if (pi.PropertyType == typeof(string) || pi.PropertyType == typeof(DateTime))
            {
                type = "varchar(255)";
                isNull = true;
            }


            var attr = pi.GetCustomAttribute<DbFieldAttribute>();
            if (attr != null)
            {
                fieldName = attr.FieldName;
                type = attr.Type;
                isNull = attr.IsNull;
            }
            return $"{fieldName} {type} {(isNull ? "null" : "not null")}";
        }


    }
}
