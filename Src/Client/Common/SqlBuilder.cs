using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using FS.Sql.ExpressionVisitor;
using FS.Sql.Infrastructure;
using FS.Sql.Internal;
using FS.Utils.Common;

namespace FS.Sql.Client.Common
{
    /// <summary>
    ///     通用SQL生成器
    /// </summary>
    public class SqlBuilder : ISqlBuilder, ISqlParam
    {
        /// <summary>
        ///     查询支持的SQL方法
        /// </summary>
        /// <param name="dbProvider">数据库提供者（不同数据库的特性）</param>
        /// <param name="expBuilder">表达式持久化</param>
        /// <param name="name">表名/视图名/存储过程名</param>
        internal SqlBuilder(AbsDbProvider dbProvider, ExpressionBuilder expBuilder, string name)
        {
            DbProvider = dbProvider;
            ExpBuilder = expBuilder;
            Name = name;
            Param = new List<DbParameter>();
            Sql = new StringBuilder();
        }

        /// <summary>
        ///     数据库提供者（不同数据库的特性）
        /// </summary>
        protected AbsDbProvider DbProvider { get; }

        /// <summary>
        ///     当前生成的SQL语句
        /// </summary>
        public StringBuilder Sql { get; private set; }

        /// <summary>
        ///     当前生成的参数
        /// </summary>
        public List<DbParameter> Param { get; }

        /// <summary>
        ///     表达式持久化
        /// </summary>
        internal ExpressionBuilder ExpBuilder { get; }

        /// <summary>
        ///     表名/视图名/存储过程名
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     提供ExpressionBinary表达式树的解析
        /// </summary>
        protected WhereVisitor WhereVisitor => DbProvider.CreateWhereVisitor(ExpBuilder.SetMap, Param);

        /// <summary>
        ///     提供ExpressionBinary表达式树的解析
        /// </summary>
        protected AssignVisitor AssignVisitor => DbProvider.CreateAssignVisitor(ExpBuilder.SetMap, Param);

        /// <summary>
        ///     提供ExpressionBinary表达式树的解析
        /// </summary>
        protected OrderByVisitor OrderByVisitor => DbProvider.CreateOrderByVisitor(ExpBuilder.SetMap, Param);

        /// <summary>
        ///     提供ExpressionBinary表达式树的解析
        /// </summary>
        protected SelectVisitor SelectVisitor => DbProvider.CreateSelectVisitor(ExpBuilder.SetMap, Param);

        /// <summary>
        ///     提供ExpressionBinary表达式树的解析
        /// </summary>
        private InsertVisitor InsertVisitor => DbProvider.CreateInsertVisitor(ExpBuilder.SetMap, Param);


        public virtual ISqlParam ToEntity()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Sql.Append($"SELECT TOP 1 {strSelectSql} FROM {DbProvider.KeywordAegis(Name)} {strWhereSql} {strOrderBySql}");
            return this;
        }

        public virtual ISqlParam ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);
            var strTopSql = top > 0 ? $"TOP {top} " : string.Empty;
            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            if (!isRand) { Sql.Append($"SELECT {strDistinctSql}{strTopSql}{strSelectSql} FROM {DbProvider.KeywordAegis(Name)} {strWhereSql} {strOrderBySql}"); }
            else if (string.IsNullOrWhiteSpace(strOrderBySql)) { Sql.Append(string.Format("SELECT {0}{1}{2}{5} FROM {3} {4} ORDER BY NEWID()", strDistinctSql, strTopSql, strSelectSql, DbProvider.KeywordAegis(Name), strWhereSql, isDistinct ? ",NEWID() as newid" : "")); }
            else
            { Sql.Append(string.Format("SELECT {2} FROM (SELECT {0} {1} *{6} FROM {3} {4} ORDER BY NEWID()) a {5}", strDistinctSql, strTopSql, strSelectSql, DbProvider.KeywordAegis(Name), strWhereSql, strOrderBySql, isDistinct ? ",NEWID() as newid" : "")); }
            return this;
        }

        public virtual ISqlParam ToList(int pageSize, int pageIndex, bool isDistinct = false)
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

            Check.IsTure(string.IsNullOrWhiteSpace(strOrderBySql) && ExpBuilder.SetMap.PhysicsMap.PrimaryFields.Count == 0, "不指定排序字段时，需要设置主键ID");

            strOrderBySql = "ORDER BY " + (string.IsNullOrWhiteSpace(strOrderBySql) ? $"{ConvertHelper.ToString(ExpBuilder.SetMap.PhysicsMap.PrimaryFields.Select(o => o.Value.Name), ",")} ASC" : strOrderBySql);
            var strOrderBySqlReverse = strOrderBySql.Replace(" DESC", " [倒序]").Replace("ASC", "DESC").Replace("[倒序]", "ASC");

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Sql.Append(string.Format("SELECT {0}TOP {2} {1} FROM (SELECT TOP {3} * FROM {4} {5} {6}) a  {7};", strDistinctSql, strSelectSql, pageSize, pageSize * pageIndex, Name, strWhereSql, strOrderBySql, strOrderBySqlReverse));
            return this;
        }

        public virtual ISqlParam Insert()
        {
            var strinsertAssemble = InsertVisitor.Visit(ExpBuilder.ExpAssign);
            Sql.Append($"INSERT INTO {Name} {strinsertAssemble}");
            return this;
        }

        public virtual ISqlParam InsertIdentity()
        {
            var strinsertAssemble = InsertVisitor.Visit(ExpBuilder.ExpAssign);
            Sql.Append($"INSERT INTO {Name} {strinsertAssemble} ");
            return this;
        }

        public virtual ISqlParam Update()
        {
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strAssemble = AssignVisitor.Visit(ExpBuilder.ExpAssign);

            // 主键如果有值、或者设置成只读条件，则自动转成条件
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Sql.Append($"UPDATE {DbProvider.KeywordAegis(Name)} SET {strAssemble} {strWhereSql}");
            return this;
        }

        public virtual ISqlParam Count(bool isDistinct = false)
        {
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Sql.Append($"SELECT {strDistinctSql}Count(0) FROM {DbProvider.KeywordAegis(Name)} {strWhereSql}");
            return this;
        }

        public virtual ISqlParam GetValue()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Sql.Append($"SELECT TOP 1 {strSelectSql} FROM {DbProvider.KeywordAegis(Name)} {strWhereSql} {strOrderBySql}");
            return this;
        }

        public virtual ISqlParam Delete()
        {
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Sql.Append($"DELETE FROM {DbProvider.KeywordAegis(Name)} {strWhereSql}");
            return this;
        }

        public virtual ISqlParam AddUp()
        {
            Check.IsTure(ExpBuilder.ExpAssign == null, "赋值的参数不能为空！");

            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strAssemble = AssignVisitor.Visit(ExpBuilder.ExpAssign);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Sql.Append($"UPDATE {DbProvider.KeywordAegis(Name)} SET {strAssemble} {strWhereSql}");
            return this;
        }

        public virtual ISqlParam Sum()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Sql.Append($"SELECT SUM({strSelectSql}) FROM {DbProvider.KeywordAegis(Name)} {strWhereSql}");
            return this;
        }

        public virtual ISqlParam Max()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Sql.Append($"SELECT MAX({strSelectSql}) FROM {DbProvider.KeywordAegis(Name)} {strWhereSql}");
            return this;
        }

        public virtual ISqlParam Min()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Sql.Append($"SELECT MIN({strSelectSql}) FROM {DbProvider.KeywordAegis(Name)} {strWhereSql}");
            return this;
        }

        #region 释放

        /// <summary>
        ///     释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        private void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                Sql.Clear();
                Sql = null;
                Param?.Clear();
            }
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}