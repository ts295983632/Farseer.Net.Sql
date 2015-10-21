using System.Linq;
using FS.Sql.Infrastructure;
using FS.Sql.Internal;
using FS.Utils.Common;

namespace FS.Sql.Client.SqlServer
{
    /// <summary>
    ///     针对SqlServer 2000 数据库 SQL生成器
    /// </summary>
    public class SqlServerSqlBuilder2000 : SqlServerSqlBuilder
    {
        /// <summary>
        ///     查询支持的SQL方法
        /// </summary>
        /// <param name="dbProvider">数据库提供者（不同数据库的特性）</param>
        /// <param name="expBuilder">表达式持久化</param>
        /// <param name="name">表名/视图名/存储过程名</param>
        internal SqlServerSqlBuilder2000(AbsDbProvider dbProvider, ExpressionBuilder expBuilder, string name) : base(dbProvider, expBuilder, name)
        {
        }

        public override ISqlParam ToList(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 不分页
            if (pageIndex == 1)
            {
                ToList(pageSize, isDistinct);
                return this;
            }

            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;

            Check.IsTure(string.IsNullOrWhiteSpace(strOrderBySql) && ExpBuilder.SetMap.PhysicsMap.PrimaryFields.Count == 0, "不指定排序字段时，需要设置主键ID");

            strOrderBySql = "ORDER BY " + (string.IsNullOrWhiteSpace(strOrderBySql) ? $"{ConvertHelper.ToString(ExpBuilder.SetMap.PhysicsMap.PrimaryFields.Select(o => o.Value.Name), ",")} ASC" : strOrderBySql);
            var strOrderBySqlReverse = strOrderBySql.Replace(" DESC", " [倒序]").Replace("ASC", "DESC").Replace("[倒序]", "ASC");

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Sql.Append(string.Format("SELECT {0} TOP {2} {1} FROM (SELECT TOP {3} {1} FROM {4} {5} {6}) a  {7};", strDistinctSql, strSelectSql, pageSize, pageSize*pageIndex, Name, strWhereSql, strOrderBySql, strOrderBySqlReverse));
            return this;
        }
    }
}