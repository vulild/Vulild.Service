using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Vulild.Service.DataBase
{
    public class WhereObj<TValue>
    {
        public virtual string GetWhere()
        {
            return "";
        }

        public WhereOperator Operator { get; set; }

        public string ColumnName { get; set; }

        public TValue Value { get; set; }

    }

    /// <summary>
    /// where操作符
    /// </summary>
    public enum WhereOperator
    {
        [Description("=")]
        Equal,

        [Description("in")]
        In,

        [Description("not in")]
        NotIn,

        [Description(">")]
        GreaterThan,

        [Description(">=")]
        GreaterThanAndEqual,

        [Description("<")]
        LessThan,

        [Description("<=")]
        LessThanAndEqual,

    }
}
