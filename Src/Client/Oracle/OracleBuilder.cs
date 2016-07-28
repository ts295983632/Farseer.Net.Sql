using FS.Sql.Infrastructure;
using FS.Sql.Internal;

namespace FS.Sql.Client.Oracle
{
    /// <summary>
    ///     针对Oracle 数据库 SQL生成器
    /// </summary>
    public class OracleBuilder : AbsSqlBuilder
    {
        /// <summary>
        ///     查询支持的SQL方法
        /// </summary>
        /// <param name="dbProvider">数据库提供者（不同数据库的特性）</param>
        /// <param name="expBuilder">表达式持久化</param>
        /// <param name="name">表名/视图名/存储过程名</param>
        internal OracleBuilder(AbsDbProvider dbProvider, ExpressionBuilder expBuilder, string name) : base(dbProvider, expBuilder, name)
        {
        }

        public override ISqlParam ToEntity()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            var strTopSql = (string.IsNullOrWhiteSpace(strWhereSql) ? "WHERE" : "AND") + " rownum <=1";

            Sql.Append($"SELECT {strSelectSql} FROM {DbProvider.KeywordAegis(Name)} {strWhereSql} {strTopSql} {strOrderBySql}");
            return this;
        }

        public override ISqlParam ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            var strTopSql = string.Empty;
            if (top > 0) { strTopSql = (string.IsNullOrWhiteSpace(strWhereSql) ? "WHERE" : "AND") + $" rownum <={top}"; }
            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            if (!isRand) { Sql.Append($"SELECT {strDistinctSql}{strSelectSql} FROM {DbProvider.KeywordAegis(Name)} {strWhereSql} {strTopSql} {strOrderBySql}"); }
            else if (string.IsNullOrWhiteSpace(strOrderBySql)) { Sql.Append(string.Format("SELECT {0}{1}{5} FROM {2} {3} {4} ORDER BY dbms_random.value", strDistinctSql, strSelectSql, DbProvider.KeywordAegis(Name), strWhereSql, strTopSql, isDistinct ? ",dbms_random.value as newid" : "")); }
            else
            { Sql.Append(string.Format("SELECT {1} FROM (SELECT {0}*{6} FROM {2} {3} {5} ORDER BY dbms_random.value) a {4}", strDistinctSql, strSelectSql, DbProvider.KeywordAegis(Name), strWhereSql, strOrderBySql, strTopSql, isDistinct ? ",dbms_random.value as newid" : "")); }
            return this;
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

            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Sql.Append(string.Format("SELECT * FROM ( SELECT A.*, ROWNUM RN FROM (SELECT {0}{1} FROM {4} {5} {6}) A WHERE ROWNUM <= {3} ) WHERE RN > {2}", strDistinctSql, strSelectSql, pageSize * (pageIndex - 1), pageSize * pageIndex, DbProvider.KeywordAegis(Name), strWhereSql, strOrderBySql));
            return this;
        }

        public override ISqlParam GetValue()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }
            var strTopSql = (string.IsNullOrWhiteSpace(strWhereSql) ? "WHERE" : "AND") + " rownum <=1";

            Sql.Append($"SELECT {strSelectSql} FROM {DbProvider.KeywordAegis(Name)} {strWhereSql} {strOrderBySql} {strTopSql}");
            return this;
        }

        public override ISqlParam InsertIdentity()
        {
            base.InsertIdentity();
            Sql.Append("SELECT @@IDENTITY ");
            return this;
        }
    }
}