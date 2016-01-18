using FS.Sql.Infrastructure;
using FS.Sql.Internal;

namespace FS.Sql.Client.OleDb
{
    /// <summary>
    ///     针对OleDb 数据库 SQL生成器
    /// </summary>
    public class OleDbBuilder : AbsSqlBuilder
    {
        /// <summary>
        ///     查询支持的SQL方法
        /// </summary>
        /// <param name="dbProvider">数据库提供者（不同数据库的特性）</param>
        /// <param name="expBuilder">表达式持久化</param>
        /// <param name="name">表名/视图名/存储过程名</param>
        internal OleDbBuilder(AbsDbProvider dbProvider, ExpressionBuilder expBuilder, string name) : base(dbProvider, expBuilder, name)
        {
        }

        public override ISqlParam ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            var strTopSql = top > 0 ? $"TOP {top}" : string.Empty;
            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            if (!isRand) { Sql.Append($"SELECT {strDistinctSql}{strTopSql} {strSelectSql} FROM {DbProvider.KeywordAegis(Name)} {strWhereSql} {strOrderBySql}"); }
            else if (string.IsNullOrWhiteSpace(strOrderBySql)) { Sql.Append(string.Format("SELECT {0}{1} {2}{5} FROM {3} {4} BY Rnd(-(TestID+\" & Rnd() & \"))", strDistinctSql, strTopSql, strSelectSql, DbProvider.KeywordAegis(Name), strWhereSql, isDistinct ? ",Rnd(-(TestID+\" & Rnd() & \")) as newid" : "")); }
            else
            { Sql.Append(string.Format("SELECT {2} FROM (SELECT {0}{1} *{6} FROM {3} {4} BY Rnd(-(TestID+\" & Rnd() & \"))) a {5}", strDistinctSql, strTopSql, strSelectSql, DbProvider.KeywordAegis(Name), strWhereSql, strOrderBySql, isDistinct ? ",Rnd(-(TestID+\" & Rnd() & \")) as newid" : "")); }
            return this;
        }

        public override ISqlParam GetValue()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Sql.Append($"SELECT TOP 1 {strSelectSql} FROM {DbProvider.KeywordAegis(Name)} {strWhereSql} {strOrderBySql}");
            return this;
        }

        public override ISqlParam InsertIdentity()
        {
            base.InsertIdentity();
            Sql.Append("SELECT @@IDENTITY;");
            return this;
        }
    }
}