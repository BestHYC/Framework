using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ORM.Dapper
{
    /// <summary>
    /// 字段对应数据库中的别名
    /// </summary>
    public class PropertyNameAttribute : Attribute
    {
        public String ColumnName { get; }
        public PropertyNameAttribute(String name)
        {
            ColumnName = name;
        }
    }
}
