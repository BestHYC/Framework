using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ORM.Dapper
{
    public sealed class SqlServerQuery<T> : Query<T> where T : IEntity
    {
        public override IBatch End()
        {
            IBatch batch = new SqlServerBatch();
            StringBuilder sb = new StringBuilder();
            String query = ConvertToQuery(Reduce.GetQuery());
            String table = ConvertToTable(Reduce.GetTable());
            String where = ConvertToWhere(Reduce.GetWhere());
            sb.Append($"select ");

            if (!String.IsNullOrEmpty(Reduce.GetTop().ColumnName))
            {
                sb.Append($" top (@{Reduce.GetTop().ColumnName}) ");
            }
            sb.Append($" {query} ");
            sb.Append($"from {table} ");
            if (!String.IsNullOrEmpty(where))
            {
                sb.Append($" where {where} ");
            }
            batch.SqlBuilder = sb.ToString();
            batch.DynamicParameters = Reduce.Parameters;
            return batch;
        }
        private String ConvertToQuery(List<ColumnRelevanceMapper> query)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var column in query)
            {
                if (sb.Length != 0)
                {
                    sb.Append(",");
                }
                if (column.TableName == null)
                {
                    sb.Append($" {column.ColumnName} ");
                    continue;
                }
                String tablename = EntityTableMapper.GetTableName(column.TableName);
                String columnname = EntityTableMapper.GetColoumName(column.TableName, column.ColumnName);
                sb.Append($" [{tablename}].[{columnname}] ");
            }
            return sb.ToString();
        }
        /// <summary>
        /// 注意,此处的ConditionConvert的写法,因为在表关联时不允许出现 on a.id = null的情况
        /// 全部通过where方式进行过滤
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        private String ConvertToTable(List<TableRelevanceMapper> tables)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var table in tables)
            {
                String tableName = EntityTableMapper.GetTableName(table.TableName);
                if (table.TableOperatorEnum == TableOperatorEnum.None)
                {
                    sb.Insert(0, $" {tableName} ");
                }
                else
                {
                    String tableOperator = table.TableOperatorEnum.GetOperator();
                    sb.Append($" {tableOperator} [{tableName}] on ");
                    foreach(var on in table.ColumnOperator)
                    {
                        sb.Append($" [{ConditionConvert(on)}] ");
                    }
                }
            }
            return sb.ToString();
        }
        private String ConditionConvert(ColumnRelevanceMapper column, bool isNull = false)
        {
            StringBuilder sb = new StringBuilder();
            //如果 null == item.id 中写法 null,那么返回"", == item.id 返回item.id is null
            //如果是 item.id == null 中写法 那么 == null 则返回 is null
            if(column.ColumnName == null && column.SqlOperatorEnum == SqlOperatorEnum.None)
            {
                return "";
            }
            if (isNull || column.ColumnName == null)
            {
                column.SqlOperatorEnum = SqlEnumConvert(column.SqlOperatorEnum);
            }
            if (column.SqlOperatorEnum == SqlOperatorEnum.EqualNotNull ||
                column.SqlOperatorEnum == SqlOperatorEnum.EqualNull)
            {
                String str = "";
                if (!String.IsNullOrEmpty(column.ColumnName))
                {
                    Type t = column.TableName;
                    String tableName = EntityTableMapper.GetTableName(t);
                    String columnName = EntityTableMapper.GetColoumName(t, column.ColumnName);
                    str = $" [{tableName}].[{columnName}] ";
                }
                return $"{str} {column.SqlOperatorEnum.GetOperator()}";
            }
            if (column.SqlOperatorEnum != SqlOperatorEnum.None)
            {
                sb.Append(column.SqlOperatorEnum.GetOperator());
            }
            //如果是 item.id = 1 中的  = 1,那么只 保存 = @ColumnName
            if (column.TableName == null)
            {
                sb.Append($" @{column.ColumnName} ");
            }
            else
            {
                Type t = column.TableName;
                String tableName = EntityTableMapper.GetTableName(t);
                String columnName = EntityTableMapper.GetColoumName(t, column.ColumnName);
                sb.Append($" [{tableName}].[{columnName}] ");
            }
            return sb.ToString();
        }
        /// <summary>
        /// 注意此处的is null 的写法,如果null != a.id,那么会返回 "" a.id is null
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        private String ConvertToWhere(List<ColumnRelevanceMapper> where)
        {
            StringBuilder sb = new StringBuilder();
            Boolean isNullOrNot = false;
            foreach(var column in where)
            {
                String str = ConditionConvert(column, isNullOrNot);
                if (String.IsNullOrEmpty(str))
                {
                    isNullOrNot = true;
                }
                else
                {
                    isNullOrNot = false;
                    sb.Append($" {str} ");
                }
            }
            return sb.ToString();
        }
        private SqlOperatorEnum SqlEnumConvert(SqlOperatorEnum sqlOperatorEnum)
        {
            if (sqlOperatorEnum == SqlOperatorEnum.Equal)
            {
                return SqlOperatorEnum.EqualNull;
            }
            if (sqlOperatorEnum == SqlOperatorEnum.NotEqual)
            {
                return SqlOperatorEnum.EqualNotNull;
            }
            return SqlOperatorEnum.None;
        }
    }
    public class SqlServerBatch : IBatch
    {
        public String SqlBuilder { get; set; }
        public DynamicParameters DynamicParameters { get; set; }
    }
}
