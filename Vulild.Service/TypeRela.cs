using System;
using System.Collections.Generic;
using System.Text;

namespace Vulild.Service
{
    /// <summary>
    /// 类型继承关系
    /// </summary>
    public class TypeRela
    {
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Type的子类
        /// </summary>
        public List<TypeRela> Children { get; set; } = new List<TypeRela>();
    }
}
